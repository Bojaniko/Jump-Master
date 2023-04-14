using System.Collections;

using UnityEngine;

using Studio28.SFX;

using JumpMaster.SFX;
using JumpMaster.Movement;
using JumpMaster.LevelControllers;
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

    public class Missile : Obstacle<MissileSO, MissileController, MissileSpawnSO, MissileSpawnMetricsSO, MissileSpawnArgs>
    {
        private Vector3 _direction;
        private Coroutine _explodeCoroutine;
        private MissileWarning _warning;

        public delegate void ExplosionEventHandler();
        public event ExplosionEventHandler OnExplode;

        protected override void Initialize()
        {
            LevelController.OnPause += Pause;
        }

        private void Pause()
        {
            c_rigidbody.velocity = Vector3.zero;
            c_animator.SetFloat("SwirlMult", 0f);
        }

        protected override void RestartInstructions()
        {

        }

        protected override void OnUpdate()
        {
            
        }

        protected override void SpawnInstructions()
        {
            switch (SpawnArgs.Direction)
            {
                case MissileDirection.UP:
                    _direction = Vector3.up;

                    transform.rotation = Quaternion.Euler(0, 0, 180f);
                    c_bounds.transform.rotation = Quaternion.identity;
                    break;

                case MissileDirection.DOWN:
                    _direction = Vector3.down;

                    // Default model rotation is down, so the rotation is 0 degrees in all angles

                    transform.rotation = Quaternion.identity;
                    c_bounds.transform.rotation = Quaternion.identity;
                    break;

                case MissileDirection.LEFT:
                    _direction = Vector3.left;

                    transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                    c_bounds.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    break;

                case MissileDirection.RIGHT:
                    _direction = Vector3.right;

                    transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    c_bounds.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
            }

            c_animator.SetFloat("SwirlMult", 1f);

            MissileWarningData warning_data = new MissileWarningData(SpawnArgs.ScreenPosition,
                SpawnArgs.Direction, SpawnData.GetCountdown());

            _warning = MissileWarning.Generate(Data.WarningInfo, warning_data);

            _warning.OnWarningEnded += EndWarning;
        }

        protected override void DespawnInstructions()
        {
            _explodeCoroutine = null;
        }

        protected override bool IsDespawnable()
        {
            if (_warning != null)
                return false;

            switch (SpawnArgs.Direction)
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
            Vector3 position_world = c_camera.ScreenToWorldPoint(SpawnArgs.ScreenPosition);
            switch (SpawnArgs.Direction)
            {
                case MissileDirection.UP:
                    position_world.y -= SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.DOWN:
                    position_world.y += SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.LEFT:
                    position_world.x += SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.RIGHT:
                    position_world.x -= SpawnMetrics.SpawnOffset;
                    break;
            }
            position_world.z = transform.position.z;
            transform.position = position_world;

            _warning.OnWarningEnded -= EndWarning;
            _warning = null;

            SFXController.Instance.PlayLoopSound<MissileThrustSFXSourceController>(Data.ThrustSFX, new MissileThrust_SFX_SC_Args(gameObject, SpawnArgs.Direction));

            gameObject.SetActive(true);
        }

        private IEnumerator Explode()
        {
            c_rigidbody.velocity = Vector3.zero;

            c_animator.SetFloat("SwirlMult", 0f);

            Instantiate(Data.ExplosionPrefab, transform.position - Vector3.forward * 0.2f, Quaternion.identity);

            SFXController.Instance.PlaySound(Data.ExplosionSFX, gameObject);

            if (OnExplode != null)
                OnExplode();

            yield return new WaitForSeconds(Data.GameObjectDestroyDelayMS / 1000f);

            Despawn();
        }

        void FixedUpdate()
        {
            if (LevelController.Paused)
                return;

            if (_explodeCoroutine == null)
                c_rigidbody.velocity = _direction * SpawnData.Speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.Equals(MovementController.Instance))
            {
                _explodeCoroutine = StartCoroutine("Explode");
            }
        }
    }
}