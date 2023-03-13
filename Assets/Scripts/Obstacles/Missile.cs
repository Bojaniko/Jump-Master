using System.Collections;

using UnityEngine;

using Studio28.SFX;

using JumpMaster.SFX;
using JumpMaster.LevelControllers;
using JumpMaster.LevelControllers.Obstacles;
using JumpMaster.UI;

namespace JumpMaster.Obstacles
{
    public enum MissileDirection { UP, DOWN, LEFT, RIGHT }

    public sealed class MissileSpawnArgs : SpawnArgs
    {
        public readonly MissileDirection Direction;
        public MissileSpawnArgs(Vector3 screen_position, MissileDirection direction) : base(screen_position)
        {
            Direction = direction;
        }
    }

    public class Missile : Obstacle, ISpawnable<Missile, MissileSpawnSO, MissileSpawnArgs>, IObstacle<MissileSO, MissileController>
    {
        private Vector3 _direction;
        private Coroutine _explodeCoroutine;
        private MissileWarning _warning;

        private ObstacleSpawnController<Missile, MissileSpawnSO, MissileSpawnArgs, ISpawnable<Missile, MissileSpawnSO, MissileSpawnArgs>> _spawnController;
        public ObstacleSpawnController<Missile, MissileSpawnSO, MissileSpawnArgs, ISpawnable<Missile, MissileSpawnSO, MissileSpawnArgs>> SpawnController
        {
            get
            {
                return _spawnController;
            }
        }
        public MissileSO Data
        {
            get
            {
                return _data as MissileSO;
            }
        }
        public MissileController Controller
        {
            get
            {
                return _controller as MissileController;
            }
        }
        public Missile ObstacleSelf
        {
            get
            {
                return this;
            }
        }

        public delegate void ExplosionEventHandler();
        public event ExplosionEventHandler OnExplode;

        protected override void Initialize()
        {
            _spawnController = new(this, this);
            _spawnController.OnSpawn += Spawn;
            _spawnController.OnDespawn += Despawn;
        }

        protected override void Pause()
        {
            _rigidbody.velocity = Vector3.zero;
            _animator.SetFloat("SwirlMult", 0f);
        }

        protected override void Unpause()
        {
            
        }

        protected override void PlayerDeath()
        {
            
        }

        protected override void Restart()
        {
            _spawnController.Despawn();
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void Spawn()
        {
            switch (_spawnController.SpawnArgs.Direction)
            {
                case MissileDirection.UP:
                    _direction = Vector3.up;

                    transform.rotation = Quaternion.Euler(0, 0, 180f);
                    _bounds.transform.rotation = Quaternion.identity;
                    break;

                case MissileDirection.DOWN:
                    _direction = Vector3.down;

                    // Default model rotation is down, so the rotation is 0 degrees in all angles

                    transform.rotation = Quaternion.identity;
                    _bounds.transform.rotation = Quaternion.identity;
                    break;

                case MissileDirection.LEFT:
                    _direction = Vector3.left;

                    transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                    _bounds.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    break;

                case MissileDirection.RIGHT:
                    _direction = Vector3.right;

                    transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    _bounds.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
            }

            _animator.SetFloat("SwirlMult", 1f);

            MissileWarningData warning_data = new MissileWarningData(_spawnController.SpawnArgs.ScreenPosition,
                _spawnController.SpawnArgs.Direction, _spawnController.SpawnData.GetCountdown());

            _warning = MissileWarning.Generate(Data.WarningInfo, warning_data);

            _warning.OnWarningEnded += EndWarning;
        }

        protected override void Despawn<ObstacleType, SpawnScriptableObject, SpawnArguments>(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn)
        {
            _explodeCoroutine = null;

            gameObject.SetActive(false);
        }

        protected override bool IsDespawnable()
        {
            if (!_spawnController.Spawned)
                return false;

            if (_warning != null)
                return false;

            switch (_spawnController.SpawnArgs.Direction)
            {
                case MissileDirection.UP:
                    return BoundsOverScreen;

                case MissileDirection.DOWN:
                    return BoundsUnderScreen;

                case MissileDirection.LEFT:
                    return BoundsLeftOfScreen;

                case MissileDirection.RIGHT:
                    return BoundsRightOfScreen;

                default:
                    return true;
            }
        }

        private void EndWarning()
        {
            Vector3 position_world = CameraController.Instance.Camera.ScreenToWorldPoint(_spawnController.SpawnArgs.ScreenPosition);
            switch (_spawnController.SpawnArgs.Direction)
            {
                case MissileDirection.UP:
                    position_world.y -= Controller.ControllerData.SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.DOWN:
                    position_world.y += Controller.ControllerData.SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.LEFT:
                    position_world.x += Controller.ControllerData.SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.RIGHT:
                    position_world.x -= Controller.ControllerData.SpawnMetrics.SpawnOffset;
                    break;
            }
            transform.position = position_world;

            _warning.OnWarningEnded -= EndWarning;
            _warning = null;

            _sfxController.PlayLoopSound<MissileThrustSFXSourceController>(Data.ThrustSFX, new MissileThrust_SFX_SC_Args(gameObject, _spawnController.SpawnArgs.Direction));

            gameObject.SetActive(true);
        }

        private IEnumerator Explode()
        {
            _rigidbody.velocity = Vector3.zero;

            _animator.SetFloat("SwirlMult", 0f);

            Instantiate(Data.ExplosionPrefab, transform.position - Vector3.forward * 0.2f, Quaternion.identity);

            SFXController.Instance.PlaySound(Data.ExplosionSFX, gameObject);

            if (OnExplode != null)
                OnExplode();

            yield return new WaitForSeconds(Data.GameObjectDestroyDelayMS / 1000f);

            _spawnController.Despawn();
        }

        void FixedUpdate()
        {
            if (LevelController.Paused)
                return;

            if (_explodeCoroutine == null)
                _rigidbody.velocity = _direction * _spawnController.SpawnData.Speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.Equals(PlayerController.Instance))
            {
                _explodeCoroutine = StartCoroutine("Explode");
            }
        }
    }
}