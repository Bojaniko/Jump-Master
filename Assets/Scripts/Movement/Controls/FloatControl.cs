using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public class FloatControl : MovementControl<FloatControlDataSO>, ITransitionable, IInputableControl
    {
        public bool ChargingJump { get; private set; }

        private float _pauseTime;

        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        public FloatControl(MovementController controller, FloatControlDataSO data) : base(controller, data)
        {
            _pauseTime = 0f;

            InputController.Instance.OnHoldStarted += StartJumpCharge;
            InputController.Instance.OnHoldPerformed += () => ChargingJump = false;
            InputController.Instance.OnHoldCancelled += CancelJumpCharge;
        }
        public override MovementState ActiveState { get { return MovementState.FLOATING; } }

        protected override bool CanStartControl()
        {
            if (ChargingJump)
                return false;
            return true;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.useGravity = false;
        }

        public override bool CanExit()
        {
            return true;
        }
        protected override void ExitControl()
        {
            _pauseTime = 0f;
        }

        public override Vector3 GetCurrentVelocity()
        {
            return Vector2.up * ControlArgs.Direction.Vertical * ControlData.Force;
        }

        public override void Pause() { }
        public override void Resume()
        {
            _pauseTime += LevelController.LastPauseDuration;
        }
        protected override void OnMovementUpdate()
        {
            if (!Started)
                return;

            if (ChargingJump)
                return;

            if (Time.time - ControlArgs.StartTime - _pauseTime < ControlData.Duration)
                return;

            if (OnTransitionable != null)
            {
                MovementControlArgs start_args = new(Controller.ControlledRigidbody, ControlArgs.Direction, 1f);
                OnTransitionable(Controller.GetControlByState(TransitionState), start_args);
            }
        }

        public MovementState TransitionState
        {
            get
            {
                return MovementState.FALLING;
            }
        }

        // Start downwards float when charging a jump.
        private void StartJumpCharge(Vector2 position, float min_hold_duration)
        {
            if (!LevelController.Started)
                return;

            if (!Controller.GetControl<ChargedJumpControl>().CanStart())
                return;

            if (OnInputDetected != null)
                OnInputDetected(this, new(Controller.ControlledRigidbody, MovementDirection.Down));
            ChargingJump = true;
        }

        private void CancelJumpCharge()
        {
            if (!LevelController.Started)
                return;

            if (OnInputDetected != null)
                OnInputDetected(Controller.GetControlByState(MovementState.FALLING), new(Controller.ControlledRigidbody, MovementDirection.Down));
            ChargingJump = false;
        }
    }
}