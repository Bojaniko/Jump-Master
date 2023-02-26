using System.Collections;

using UnityEngine;

using JumpMaster.LevelControllers;
using JumpMaster.LevelControllers.Obstacles;

using Studio28.SFX;

namespace JumpMaster.Obstacles
{
    public sealed class FallingBombArgs : SpawnArgs
    {
        public readonly Vector3 SpawnPosition;
        public readonly int SpawnPositionOrder;

        public FallingBombArgs(Vector3 screen_position, Vector3 spawn_position, int spawn_position_order) : base(screen_position)
        {
            SpawnPosition = spawn_position;
            SpawnPositionOrder = spawn_position_order;
        }
    }

    public class FallingBomb : Obstacle, ISpawnable<FallingBomb, FallingBombSpawnSO, FallingBombArgs>, IObstacle<FallingBombSO, FallingBombController>
    {
        private float _armingDuration;
        private float _gameObjectDestroyDuration;

        private float _currentDetectionShowTime;
        private float _detectionShowDuration;
        private float _detectionShowDistanceScreen;

        private Renderer _detectionRenderer;

        private Material _bombLightRef;

        private Coroutine _explodeCoroutine;

        private ObstacleSpawnController<FallingBomb, FallingBombSpawnSO, FallingBombArgs, ISpawnable<FallingBomb, FallingBombSpawnSO, FallingBombArgs>> _spawnController;
        public ObstacleSpawnController<FallingBomb, FallingBombSpawnSO, FallingBombArgs, ISpawnable<FallingBomb, FallingBombSpawnSO, FallingBombArgs>> SpawnController
        {
            get
            {
                return _spawnController;
            }
        }
        public FallingBombSO Data
        {
            get
            {
                return _data as FallingBombSO;
            }
        }
        public FallingBombController Controller
        {
            get
            {
                return _controller as FallingBombController;
            }
        }
        public FallingBomb ObstacleSelf
        {
            get
            {
                return this;
            }
        }

        protected override void Spawn()
        {
            transform.position = _spawnController.SpawnArgs.SpawnPosition;

            _sphereCol.radius = _spawnController.SpawnData.DetectionRadius;
            _detectionRenderer.transform.localScale = Vector3.one * (1 / _data.Scale) * _spawnController.SpawnData.DetectionRadius;

            _currentDetectionShowTime = _detectionShowDuration;
            _detectionShowDistanceScreen = Vector2.Distance(Camera.main.WorldToScreenPoint(Vector2.zero), Camera.main.WorldToScreenPoint(new Vector2(0, _spawnController.SpawnData.DetectionShowDistance)));

            _armingDuration = _spawnController.SpawnData.ArmingDurationMS / 1000f;

            _bombLightRef.SetColor("_Color", Data.UnarmedLightColor);
            _detectionRenderer.material.SetFloat("_Transparency", 1f);

            gameObject.SetActive(true);
        }

        protected override void Despawn<ObstacleType, SpawnScriptableObject, SpawnArguments>(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn)
        {
            if (_explodeCoroutine != null)
                _explodeCoroutine = null;

            gameObject.SetActive(false);
        }

        protected override bool IsDespawnable()
        {
            if (!_spawnController.Spawned)
                return false;
            if (_explodeCoroutine != null) return false;
            return BoundsUnderScreen;
        }

        private IEnumerator Explode()
        {
            _sfxController.PlaySound(Data.ArmingBeepSound, gameObject);
            _bombLightRef.SetColor("_Color", Data.ArmedLightColor);

            yield return new WaitForSeconds(_armingDuration);

            _sfxController.PlaySound(Data.ArmingBeepSound, gameObject);
            _animator.Play("explode", 1);

            yield return new WaitForSeconds(_armingDuration);

            _sfxController.PlaySound(Data.ExplosionSound, gameObject);
            Instantiate(Data.ExplosionPrefab, transform.position - Vector3.forward * 0.2f, Quaternion.identity);

            yield return new WaitForSeconds(_gameObjectDestroyDuration);

            _spawnController.Despawn();
        }

        private void ShowDetection()
        {
            _currentDetectionShowTime -= 1f * Time.deltaTime;

            if (_currentDetectionShowTime <= 0.0f)
                _currentDetectionShowTime = 0.0f;

            _detectionRenderer.material.SetFloat("_Transparency", _currentDetectionShowTime / _detectionShowDuration);
        }

        private void HideDetection()
        {
            _currentDetectionShowTime += 1f * Time.deltaTime;

            if (_currentDetectionShowTime >= _detectionShowDuration)
                _currentDetectionShowTime = _detectionShowDuration;

            _detectionRenderer.material.SetFloat("_Transparency", _currentDetectionShowTime / _detectionShowDuration);
        }

        protected override void Initialize()
        {
            _detectionShowDuration = Data.DetectionShowDurationMS / 1000f;
            _gameObjectDestroyDuration = Data.GameObjectDestroyDelayMS / 1000f;

            for (int child = 0; child < transform.childCount; child++)
            {
                if (transform.GetChild(child).name.Equals("bomb"))
                {
                    foreach (Material material in transform.GetChild(child).GetComponent<Renderer>().materials)
                    {
                        if (material.name.ToUpper().Equals("LIGHT (INSTANCE)"))
                        {
                            _bombLightRef = material;
                            break;
                        }
                    }
                }
                if (transform.GetChild(child).name.Equals("detection"))
                {
                    _detectionRenderer = transform.GetChild(child).GetComponent<Renderer>();
                    _detectionRenderer.material.SetFloat("_Transparency", _currentDetectionShowTime / _detectionShowDuration);
                }
            }

            _spawnController = new(this, this);
            _spawnController.OnSpawn += Spawn;
            _spawnController.OnDespawn += Despawn;
        }

        protected override void Pause()
        {

        }

        protected override void Unpause()
        {

        }

        protected override void PlayerDeath()
        {

        }

        protected override void Restart()
        {

        }

        protected override void OnUpdate()
        {
            if (_explodeCoroutine == null)
            {
                if (Vector2.Distance(ScreenPosition, Camera.main.WorldToScreenPoint(LevelController.Instance.PlayerGameObject.transform.position)) <= _detectionShowDistanceScreen)
                {
                    if (_currentDetectionShowTime > 0.0f) ShowDetection();
                }
                else
                {
                    if (_currentDetectionShowTime < _detectionShowDuration) HideDetection();
                }
            }
            else
            {
                if (_currentDetectionShowTime < _detectionShowDuration) HideDetection();
            }
        }

        private void FixedUpdate()
        {
            if (_explodeCoroutine == null)
                _rigidbody.velocity = Vector3.down * _animator.GetFloat("Falling");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _explodeCoroutine = StartCoroutine("Explode");
            }
        }
    }
}