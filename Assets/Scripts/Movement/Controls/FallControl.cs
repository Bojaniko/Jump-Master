using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class FallControl : MovementControl<FallControlDataSO>, IInputableControl
    {
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
        }

        public override Vector3 GetCurrentVelocity()
        {
            return Controller.ControlledRigidbody.velocity;
        }

        public override void Pause() { }

        public override void Resume() { }

        protected override void OnMovementUpdate() { }

        private void JumpChargeCancelInput()
        {
            if (!Controller.ActiveControl.ActiveState.Equals(MovementState.FALLING)
                && !Controller.ActiveControl.ActiveState.Equals(MovementState.JUMP_CHARGING))
                return;

            OnInputDetected(this, ControlArgs);
        }
    }
}