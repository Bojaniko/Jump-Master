using UnityEngine;

using JumpMaster.LevelControllers;
using JumpMaster.Controls;

namespace JumpMaster.Movement
{
    public class ChargedJumpControl : MovementControl<ChargedJumpControlDataSO>, ITransitionable, IInputableControl
    {
        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        private float _chargeStartTime;

        private readonly JumpControl _jumpControl;

        public ChargedJumpControl(MovementController controller, ChargedJumpControlDataSO data, JumpControl jump_control) : base(controller, data)
        {
            _jumpControl = jump_control;

            InputController.Instance.OnHoldStarted += StartJumpCharge;
            InputController.Instance.OnHoldPerformed += PerformJumpCharge;
        }
        public override MovementState ActiveState { get { return MovementState.JUMP_CHARGING; } }

        public override bool CanExit()
        {
            if (MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.JUMPING))
            {
                if ((MovementController.Instance.gameObject.transform.position.y - ControlArgs.StartPosition.y) / (ControlData.ChargedHeight * ControlArgs.Strength) < ControlData.MinChainHeight)
                    return false;
            }
            return true;
        }
        protected override void ExitControl() { }

        protected override bool CanStartControl()
        {
            if (!LevelController.Started)
                return false;

            if (Controller.ActiveControl.ActiveState.Equals(MovementState.HANGING))
                return false;

            if (Controller.ActiveControl.ActiveState.Equals(MovementState.JUMPING))
            {
                if (_jumpControl.Chain >= _jumpControl.ControlData.MaxChain) return false;
            }

            return true;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.useGravity = false;
        }

        public override Vector3 GetCurrentVelocity()
        {
            float heightPercentage = (Controller.transform.position.y - ControlArgs.StartPosition.y) / (ControlData.ChargedHeight * ControlArgs.Strength);
            return Vector3.Lerp(Vector3.up * (ControlData.ChargedForce * ControlArgs.Strength), Vector3.zero, heightPercentage);
        }

        public MovementState TransitionState
        {
            get
            {
                return MovementState.FALLING;
            }
        }

        public override void Pause() { }
        public override void Resume() { }
        protected override void OnMovementUpdate()
        {
            if (!Started)
                return;

            if (Controller.ControlledRigidbody.velocity.y > ControlData.EndForce)
                return;

            if (OnTransitionable != null)
            {
                MovementControlArgs start_args = new(Controller.ControlledRigidbody, ControlArgs.Direction, 1f);
                OnTransitionable(Controller.GetControlByState(TransitionState), start_args);
            }
        }

        private void StartJumpCharge(Vector2 position, float min_hold_duration)
        {
            _chargeStartTime = Time.time + min_hold_duration;
        }

        private void PerformJumpCharge()
        {
            if (!MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.FLOATING))
                return;

            float duration_percentage = (Time.time - _chargeStartTime) / ControlData.MaxChargeDuration;

            if (duration_percentage == 0f)
                return;

            duration_percentage = Mathf.Clamp(duration_percentage, ControlData.MinChargePercentage, 1f);

            if (OnInputDetected != null)
                OnInputDetected(this, new(Controller.ControlledRigidbody, MovementDirection.Up, duration_percentage));
        }
    }
}