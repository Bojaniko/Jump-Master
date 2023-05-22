using System.Collections;

using UnityEngine;

using JumpMaster.Movement;

namespace JumpMaster.Obstacles
{
    public class ElectroBall : Obstacle<ElectroBallSO, ElectroBallSpawnSO, ElectroBallSpawnMetricsSO, SpawnArgs>
    {
        protected override void Initialize()
        {

        }

        private Coroutine _laserCycle;
        private IEnumerable ShootLaserCycle()
        {
            yield return new WaitUntil(IsBallOpen);
        }

        private bool IsBallOpen()
        {
            return c_animator.GetBool("Opened");
        }


        private bool _completedInterceptions = false;

        protected override void SpawnInstructions()
        {
            _completedInterceptions = false;
        }

        protected override void DespawnInstructions()
        {

        }
        protected override bool IsDespawnable()
        {
            if (_completedInterceptions)
                return false;
            return BoundsUnderScreen;
        }

        protected override void OnFixedUpdate()
        {
            if (_laserCycle != null)
                return;

            if (!_completedInterceptions)
                c_rigidbody.velocity = GetVelocityTowardsPlayer();
            else
                c_rigidbody.velocity = Vector2.down * Data.EscapeMovementSpeed;
        }
        
        private Vector2 GetVelocityTowardsPlayer()
        {
            Vector3 targetPosition = MovementController.Instance.transform.position;
            targetPosition.y += Data.StopPositionFromPlayer;
            return (transform.position - targetPosition).normalized * SpawnData.MovementSpeed;
        }

        protected override void OnUpdate()
        {

        }
    }
}