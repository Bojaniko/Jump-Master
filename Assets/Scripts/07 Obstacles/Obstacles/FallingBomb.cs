using System.Collections;

using UnityEngine;

using JumpMaster.Movement;
using JumpMaster.Damage;
using JumpMaster.Core;

using Studio28.Audio;

namespace JumpMaster.Obstacles
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class FallingBomb : Obstacle<FallingBombSO, FallingBombSpawnSO>
    {
        protected override void Initialize()
        {
            LevelManager.OnPause += Pause;
            LevelManager.OnResume += Resume;
            LevelManager.OnEndLevel += EndLevel;

            CacheParts();

            c_detectionCol.isTrigger = true;
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
            StartExplosion();
        }

        protected override void OnUpdate()
        {
            Detection();
        }

        protected override void OnFixedUpdate()
        {
            if (LevelManager.Paused)
                return;

            if (_explodeCoroutine != null)
                return;

            c_rigidbody.velocity = Vector2.down * c_animator.GetFloat("Falling");
        }

        // ##### SPAWNING ##### \\
        
        protected override void SpawnInstructions()
        {
            transform.position = GetWorldSpawnPosition(SpawnArguments.ScreenPosition);

            ResetDetection(SpawnData);
            ResetExplosion(SpawnData);
        }

        protected override void DespawnInstructions()
        {
            if (_explodeCoroutine != null)
            {
                StopCoroutine(_explodeCoroutine);
                _explodeCoroutine = null;
            }
        }

        protected override bool IsDespawnable()
        {
            if (_explodeCoroutine != null) return false;
            return BoundsUnderScreen;
        }

        private Vector3 GetWorldSpawnPosition(Vector3 screen_position)
        {
            Vector2 pos_screen_margined = screen_position;
            pos_screen_margined.y += (Bounding.ScreenMax.y - Bounding.ScreenMin.y);

            Vector3 pos_world = c_camera.ScreenToWorldPoint(pos_screen_margined);
            pos_world.z = Data.Z_Position;

            return pos_world;
        }

        // ##### DETECTION CIRCLE ##### \\

        private float _currentDetectionShowTime;
        private float _detectionShowDistanceScreen;

        private void ResetDetection(FallingBombSpawnSO spawn_data)
        {
            _currentDetectionShowTime = c_detectionShowDuration;
            _detectionShowDistanceScreen = Vector2.Distance(Vector2.zero, c_camera.WorldToScreenPoint(new Vector2(0, spawn_data.DetectionShowDistance)));

            c_detectionCol.radius = spawn_data.DetectionRadius;
            c_detectionRenderer.material.SetFloat("_Transparency", 1f);
            c_detectionRenderer.transform.localScale = (Vector3.one * c_invertedScale) * spawn_data.DetectionRadius;
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

        private void Detection()
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

        // ##### EXPLOSION ##### \\

        private float _armingDuration;
        private Coroutine _explodeCoroutine;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                StartExplosion();
            }
        }

        private void ResetExplosion(FallingBombSpawnSO spawn_data)
        {
            _armingDuration = spawn_data.ArmingDuration / 1000f;

            c_bombLightRef.SetColor("_EmissionColor", Data.UnarmedLightColor);
        }

        private void StartExplosion()
        {
            if (!gameObject.activeSelf)
                return;

            if (_explodeCoroutine != null)
                return;

            _explodeCoroutine = StartCoroutine("Explode");
        }

        private IEnumerator Explode()
        {
            AudioController.Instance.PlaySound(Data.ArmingBeepSound, gameObject);
            c_bombLightRef.SetColor("_EmissionColor", Data.ArmedLightColor);

            yield return new WaitForSecondsPausable(_armingDuration);

            AudioController.Instance.PlaySound(Data.ArmingBeepSound, gameObject);
            c_animator.Play("explode", 1);

            yield return new WaitForSecondsPausable(_armingDuration);

            AudioController.Instance.PlaySound(Data.ExplosionSound, gameObject);

            c_explosionEffect.Play(SpawnData.ExplosionData, transform.position - Vector3.forward * 0.2f, Data.Scale);

            ExplosionData explosionData = new(SpawnData.ExplosionData.Radius * Data.Scale,
                SpawnData.ExplosionData.Duration,
                transform.position,
                SpawnData.ExplosionData.Damage,
                "Player");
            ExplosionDamageSource explosionSource = new(explosionData);
            DamageController.Instance.LogSource(explosionSource);

            yield return new WaitForSeconds(c_gameObjectDestroyDuration);

            Despawn();
        }

        // ##### CACHE ##### \\

        private Material c_bombLightRef;
        private Renderer c_detectionRenderer;
        private CircleCollider2D c_detectionCol;

        private ExplosionEffect c_explosionEffect;

        private float c_detectionShowDuration;
        private float c_gameObjectDestroyDuration;

        private void CacheParts()
        {
            c_detectionShowDuration = Data.DetectionShowDuration / 1000f;
            c_gameObjectDestroyDuration = Data.ExplosionDestroyDelay / 1000f;

            c_explosionEffect = ExplosionEffect.Generate(Data.ExplosionPrefab);

            c_detectionCol = GetComponent<CircleCollider2D>();
            c_detectionCol.isTrigger = true;

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
                if (transform.GetChild(child).name.Equals("parachute"))
                {
                    foreach (Material material in transform.GetChild(child).GetComponent<Renderer>().materials)
                        material.SetFloat("_Scale", Data.Scale);
                }
            }
        }
    }
}