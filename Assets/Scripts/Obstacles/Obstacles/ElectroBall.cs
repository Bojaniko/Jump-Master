using System.Collections;

using UnityEngine;

using JumpMaster.Movement;
using JumpMaster.Core;

namespace JumpMaster.Obstacles
{
    public class ElectroBall : Obstacle<ElectroBallSO, ElectroBallSpawnSO, ElectroBallSpawnMetricsSO, SpawnArgs>
    {
        protected override void Initialize()
        {
            Cache();

            _targetPositionLocal = new Vector2(0, Data.StopPositionFromPlayer);

            LevelManager.OnPause += Pause;
            LevelManager.OnResume += Resume;
        }

        // ##### GAME STATE ##### \\

        private void Pause()
        {
            c_animator.SetFloat("PauseMultiplier", 0f);
        }

        private void Resume()
        {
            c_animator.SetFloat("PauseMultiplier", 1f);
        }

        // ##### LASER CYCLE ##### \\

        private int _interceptions = 0;
        private bool _completedInterceptions = false;

        private Coroutine _laserCycle;

        private IEnumerator ShootLaserCycle()
        {
            c_rigidbody.velocity = Vector2.zero;

            c_animator.Play("open");

            yield return new WaitUntil(IsBallOpen);

            ShootLaser();

            yield return new WaitForSecondsPausable(SpawnData.LaserHoldDuration);

            DisableLaser();
            c_animator.SetBool("Closing", true);

            yield return new WaitUntil(IsBallClosed);

            yield return new WaitForSecondsPausable(Data.PostCloseDelay);

            EndLaserCycle();
        }
        private void EndLaserCycle()
        {
            StopCoroutine(_laserCycle);
            _laserCycle = null;
            _interceptions++;

            c_animator.SetBool("Closing", false);

            if (_interceptions == SpawnData.Interceptions)
                _completedInterceptions = true;
        }

        private void ShootLaser()
        {
            c_laser.enabled = true;
        }

        private void DisableLaser()
        {
            c_laser.enabled = false;
        }

        private bool IsBallOpen()
        {
            if (!c_animator.GetCurrentAnimatorStateInfo(0).IsName("open"))
                return false;
            return c_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f;
        }
        private bool IsBallClosed()
        {
            if (!c_animator.GetCurrentAnimatorStateInfo(0).IsName("close"))
                return false;
            return c_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f;
        }

        // ##### SPAWNING ##### \\

        private Vector2 _targetPositionLocal;

        protected override void SpawnInstructions()
        {
            transform.position = GetWorldSpawnPosition(SpawnArgs.ScreenPosition);

            _interceptions = 0;
            _completedInterceptions = false;

            c_laser.enabled = false;
            c_animator.SetFloat("PauseMultiplier", 1f);
            c_animator.SetBool("Closing", false);
        }
        protected override void DespawnInstructions()
        {

        }
        protected override bool IsDespawnable()
        {
            if (!_completedInterceptions)
                return false;
            return BoundsUnderScreen;
        }

        private Vector3 GetWorldSpawnPosition(Vector3 screen_position)
        {
            // TODO: margin ball by it's bounds size

            Vector3 pos_world = c_camera.ScreenToWorldPoint(screen_position);
            pos_world.z = Data.Z_Position;

            return pos_world;
        }

        // ##### MOVEMENT ##### \\

        protected override void OnFixedUpdate()
        {
            if (_laserCycle != null)
                return;

            if (!_completedInterceptions)
                c_rigidbody.velocity = GetDirectionVector() * SpawnData.MovementSpeed;
            else
                c_rigidbody.velocity = Vector2.down * Data.EscapeMovementSpeed;
        }

        private Vector2 GetDirectionVector()
        {
            Vector2 target = c_player.transform.TransformPoint(_targetPositionLocal);
            Vector2 current = transform.position;
            Vector2 direction = (target - current).normalized;
            //Debug.Log($"Target position {target}, current position {current}. Direction is {direction}.");
            return direction;
        }

        protected override void OnUpdate()
        {
            if (_completedInterceptions)
                return;

            if (_laserCycle != null)
                return;

            if (Vector2.Distance(transform.position, c_player.transform.TransformPoint(_targetPositionLocal)) > 0.5f)
                return;

            _laserCycle = StartCoroutine(ShootLaserCycle());
        }

        // ##### CACHE ##### \\

        private GameObject c_player;

        private LineRenderer c_laser;

        private void Cache()
        {
            c_laser = GetComponent<LineRenderer>();

            c_player = MovementController.Instance.gameObject;
        }
    }
}