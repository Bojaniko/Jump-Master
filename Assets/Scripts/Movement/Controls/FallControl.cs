using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class FallControl : MovementControl<FallControlDataSO, FallControlArgs>, IInputableControl
    {
        private Vector3 _prePauseVelocity;

        public FallControl(MovementController controller, FallControlDataSO data) : base(controller, data)
        {
            InputController.Instance.OnHoldCancelled += JumpChargeCancelInput;
        }

        public override MovementState ActiveState { get { return MovementState.FALLING; } }

        public event ControlInputEventHandler OnInputDetected;

        public override bool CanExit()
        {
            return true;
        }
        protected override void ExitControl() { }

        protected override bool CanStartControl()
        {
            return LevelController.Started;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.useGravity = true;
            Controller.ControlledRigidbody.drag = _controlArgs.Drag;
        }

        public override Vector3 GetCurrentVelocity()
        {
            if (_prePauseVelocity.Equals(Vector3.zero))
                return Controller.ControlledRigidbody.velocity;
            Vector3 velocity = _prePauseVelocity;
            _prePauseVelocity = Vector3.zero;
            return velocity;
        }

        public override void Pause()
        {
            _prePauseVelocity = Controller.ControlledRigidbody.velocity;
        }

        public override void Resume() { }

        protected override void OnMovementUpdate() { }

        private void JumpChargeCancelInput()
        {
            if (Controller.ActiveControl.ActiveState.Equals(MovementState.HANGING))
                return;
            OnInputDetected?.Invoke(this, new FallControlArgs(new(Controller)));
        }
    }
}