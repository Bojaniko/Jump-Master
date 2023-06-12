using JumpMaster.Core;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class FallControl : MovementControl<FallControlDataSO, MovementControlArgs>, IInputableControl
    {
        private Vector2 _prePauseVelocity;

        public event ControlInputEventHandler OnInputDetected;

        public FallControl(MovementController controller, FallControlDataSO data) : base(controller, data)
        {
        }

        public override MovementState ActiveState { get { return MovementState.FALLING; } }

        public override bool CanExit(IMovementControl exit_control) => true;
        protected override void ExitControl() { _drag = 0f; }
        protected override bool CanStartControl() => LevelManager.Started;

        protected override void StartControl()
        {
            Controller.ControlledRigidbody.gravityScale = 1f;

            _drag = ControlData.Drag;
            _dragVelocityLimit = _drag / ((Mathf.Abs(Controller.ControlledRigidbody.velocity.x) + Mathf.Abs(Controller.ControlledRigidbody.velocity.y)) * 0.5f);
        }

        public override void Pause()
        {
            _prePauseVelocity = Controller.ControlledRigidbody.velocity;
        }
        public override void Resume() { }

        protected override void OnMovementUpdate() { }

        // ##### PHYSICS ##### \\

        private float _drag;
        private float _dragVelocityLimit;

        public override Vector2 GetCurrentVelocity()
        {
            Vector2 velocity = Controller.ControlledRigidbody.velocity;

            if (_prePauseVelocity != Vector2.zero)
            {
                velocity = _prePauseVelocity;
                _prePauseVelocity = Vector2.zero;
            }

            if (_drag != 0f)
            {
                velocity = DampenVelocity(velocity, _drag, _dragVelocityLimit);
                if (VelocityDampened(velocity, _dragVelocityLimit))
                    _drag = 0f;
            }

            return velocity;
        }

        private Vector2 DampenVelocity(Vector2 velocity, float damp, float damp_limit)
        {
            if (velocity.x != 0f)
            {
                if (velocity.x < -damp_limit)
                {
                    velocity.x += damp * Time.fixedDeltaTime;
                    if (velocity.x > -damp_limit)
                        velocity.x = -damp_limit;
                }
                else if (velocity.x > damp_limit)
                {
                    velocity.x -= damp * Time.fixedDeltaTime;
                    if (velocity.x < damp_limit)
                        velocity.x = damp_limit;
                }
            }
            if (velocity.y > Physics2D.gravity.y)
            {
                velocity.y -= damp * Time.fixedDeltaTime;
                if (velocity.y < Physics2D.gravity.y)
                    velocity.y = Physics2D.gravity.y;
            }
            return velocity;
        }

        private bool VelocityDampened(Vector2 velocity, float damp_limit)
        {
            if (velocity.x > damp_limit || velocity.x < -damp_limit)
                return false;
            if (velocity.y > damp_limit)
                return false;
            return true;
        }

        // ##### INPUT ##### \\
        
        private void JumpChargeCancelInput()
        {
            if (Controller.ActiveControl.ActiveState.Equals(MovementState.HANGING))
                return;
            OnInputDetected?.Invoke(this, new(Controller));
        }
    }
}