using System.Collections;

using UnityEngine;

using JumpMaster.Structure;
using JumpMaster.Movement;
using JumpMaster.LevelControllers;

using Studio28.SFX;

namespace JumpMaster.Obstacles
{
    public sealed class FallingBombArgs : SpawnArgs
    {
        public readonly int SpawnPositionOrder;

        public FallingBombArgs(Vector3 screen_position, int spawn_position_order) : base(screen_position)
        {
            SpawnPositionOrder = spawn_position_order;
        }
    }

    public class FallingBomb : Obstacle<FallingBombSO, FallingBombController, FallingBombSpawnSO, FallingBombSpawnMetricsSO, FallingBombArgs>
    {
        private float _armingDuration;
        private float _currentDetectionShowTime;
        private float _detectionShowDistanceScreen;

        private Coroutine _explodeCoroutine;

        protected override void SpawnInstructions()
        {
            _currentDetectionShowTime = c_detectionShowDuration;
            c_bombLightRef.SetColor("_Color", Data.UnarmedLightColor);
            c_detectionRenderer.material.SetFloat("_Transparency", 1f);

            Vector2 pos_screen_margined = SpawnArgs.ScreenPosition;
            pos_screen_margined.y += (BoundsScreenPosition.max.y - BoundsScreenPosition.min.y);
            Vector3 pos_world = c_camera.ScreenToWorldPoint(pos_screen_margined);
            pos_world.z = Data.Z_Position;
            transform.position = pos_world;

            c_sphereCol.radius = SpawnData.DetectionRadius;
            c_detectionRenderer.transform.localScale = Vector3.one * (1 / Data.Scale) * SpawnData.DetectionRadius;
            _detectionShowDistanceScreen = Vector2.Distance(c_camera.WorldToScreenPoint(Vector2.zero), c_camera.WorldToScreenPoint(new Vector2(0, SpawnData.DetectionShowDistance)));
            _armingDuration = SpawnData.ArmingDurationMS / 1000f;

            gameObject.SetActive(true);
        }

        protected override void DespawnInstructions()
        {
            if (_explodeCoroutine != null)
                _explodeCoroutine = null;

            gameObject.SetActive(false);
        }

        protected override bool IsDespawnable()
        {
            if (_explodeCoroutine != null) return false;
            return BoundsUnderScreen;
        }

        private IEnumerator Explode()
        {
            SFXController.Instance.PlaySound(Data.ArmingBeepSound, gameObject);
            c_bombLightRef.SetColor("_Color", Data.ArmedLightColor);

            yield return new WaitForSecondsPausable(_armingDuration);

            SFXController.Instance.PlaySound(Data.ArmingBeepSound, gameObject);
            c_animator.Play("explode", 1);

            yield return new WaitForSecondsPausable(_armingDuration);

            SFXController.Instance.PlaySound(Data.ExplosionSound, gameObject);
            Instantiate(Data.ExplosionPrefab, transform.position - Vector3.forward * 0.2f, Quaternion.identity);
            DamageController.LogDamage(new DamageInfo(transform.position, SpawnData.DetectionRadius, 30f));

            yield return new WaitForSeconds(c_gameObjectDestroyDuration);

            Despawn();
        }

        private void ShowDetection()
        {
            _currentDetectionShowTime -= 1f * Time.deltaTime;

            if (_currentDetectionShowTime <= 0.0f)
                _currentDetectionShowTime = 0.0f;

            c_detectionRenderer.material.SetFloat("_Transparency", _currentDetectionShowTime / c_detectionShowDuration);
        }

        private void HideDetection()
        {
            _currentDetectionShowTime += 1f * Time.deltaTime;

            if (_currentDetectionShowTime >= c_detectionShowDuration)
                _currentDetectionShowTime = c_detectionShowDuration;

            c_detectionRenderer.material.SetFloat("_Transparency", _currentDetectionShowTime / c_detectionShowDuration);
        }

        protected override void Initialize()
        {
            LevelController.OnPause += Pause;
            LevelController.OnResume += Resume;
            LevelController.OnEndLevel += EndLevel;

            CacheParts();
        }

        private void Pause()
        {
            c_rigidbody.velocity = Vector3.zero;
            c_animator.SetFloat("Speed", 0f);
        }

        private void Resume()
        {
            c_animator.SetFloat("Speed", 1f);
        }

        private void EndLevel()
        {
            if (!gameObject.activeSelf)
                return;

            _explodeCoroutine = StartCoroutine("Explode");
        }

        protected override void RestartInstructions()
        {
            _currentDetectionShowTime = c_detectionShowDuration;
            c_bombLightRef.SetColor("_Color", Data.UnarmedLightColor);
            c_detectionRenderer.material.SetFloat("_Transparency", 1f);
        }

        protected override void OnUpdate()
        {
            if (_explodeCoroutine == null)
            {
                if (Vector2.Distance(ScreenPosition, c_camera.WorldToScreenPoint(MovementController.Instance.transform.position)) <= _detectionShowDistanceScreen)
                {
                    if (_currentDetectionShowTime > 0.0f) ShowDetection();
                }
                else
                {
                    if (_currentDetectionShowTime < c_detectionShowDuration) HideDetection();
                }
            }
            else
            {
                if (_currentDetectionShowTime < c_detectionShowDuration) HideDetection();
            }
        }

        private void FixedUpdate()
        {
            if (LevelController.Paused)
                return;

            if (_explodeCoroutine != null)
                return;

            c_rigidbody.velocity = Vector3.down * c_animator.GetFloat("Falling");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _explodeCoroutine = StartCoroutine("Explode");
            }
        }

        // ##### CACHE ##### \\

        private Material c_bombLightRef;
        private Renderer c_detectionRenderer;

        private float c_detectionShowDuration;
        private float c_gameObjectDestroyDuration;

        private void CacheParts()
        {
            c_detectionShowDuration = Data.DetectionShowDurationMS / 1000f;
            c_gameObjectDestroyDuration = Data.GameObjectDestroyDelayMS / 1000f;

            for (int child = 0; child < transform.childCount; child++)
            {
                if (transform.GetChild(child).name.Equals("bomb"))
                {
                    foreach (Material material in transform.GetChild(child).GetComponent<Renderer>().materials)
                    {
                        if (material.name.ToUpper().Equals("LIGHT (INSTANCE)"))
                        {
                            c_bombLightRef = material;
                            break;
                        }
                    }
                }
                if (transform.GetChild(child).name.Equals("detection"))
                {
                    c_detectionRenderer = transform.GetChild(child).GetComponent<Renderer>();
                    c_detectionRenderer.material.SetFloat("_Transparency", _currentDetectionShowTime / c_detectionShowDuration);
                }
            }
        }
    }
}