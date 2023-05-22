using UnityEngine;

using JumpMaster.Damage;

namespace JumpMaster.Movement
{
    public class StillControl : MovementControl<StillControlDataSO, MovementControlArgs>, IExplicitControl
    {
        public StillControl(MovementController controller, StillControlDataSO data) : base(controller, data)
        {
            _stunned = false;
            _stunDuration = 0f;
            _stunStartTime = 0f;

            DamageController.Instance.RegisterListener<StunAreaDamageSource>(Controller.gameObject, StunDetection);
        }

        public override MovementState ActiveState { get { return MovementState.STILL; } }

        public event ExplicitControlEventHandler OnExplicitDetection;

        public override bool CanExit(IMovementControl exit_control)
        {
            return !_stunned;
        }
        protected override bool CanStartControl()
        {
            return true;
        }

        public override Vector2 GetCurrentVelocity()
        {
            return Vector2.zero;
        }

        public override void Pause()
        {

        }

        public override void Resume()
        {

        }

        protected override void OnMovementUpdate()
        {
            if (!Started && !_stunned)
                return;
            if (Time.time - _stunStartTime < _stunDuration)
                return;

            _stunned = false;
            MovementControlArgs args = new(Controller);
            OnExplicitDetection?.Invoke(Controller.GetControl<FallControl>(), args);
        }

        protected override void StartControl()
        {
            Controller.ControlledRigidbody.gravityScale = 0f;
            Controller.ControlledRigidbody.velocity = Vector3.zero;
        }
        protected override void ExitControl()
        {
            _stunned = false;
        }
        
        // ##### STUNNING ##### \\

        private bool _stunned;
        private float _stunDuration;
        private float _stunStartTime;
        private void StunDetection(IDamageRecord record)
        {
            _stunDuration = record.Duration;
            _stunStartTime = Time.time;
            _stunned = true;
            Controller.ControlledRigidbody.transform.position = new Vector2(Controller.ControlledRigidbody.transform.position.x, record.SourcePosition.y);

            if (Started)
                return;

            MovementControlArgs args = new(Controller);
            OnExplicitDetection?.Invoke(this, args);
        }
    }
}