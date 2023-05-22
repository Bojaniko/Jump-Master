using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public class FloatControl : MovementControl<FloatControlDataSO, FloatControlArgs>, ITransitionable, IInputableControl, IDirectional
    {
        public bool ChargingJump { get; private set; }

        private float _pauseTime;

        private Vector2 _direction;

        public MovementDirection Direction
        {
            get
            {
                if (_controlArgs == null)
                    return MovementDirection.Zero;
                return _controlArgs.Direction;
            }
        }

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
            Controller.ControlledRigidbody.gravityScale = 0f;

            _direction = Vector2.up * _controlArgs.Direction.Vertical * ControlArgs.Strength;
        }

        public override bool CanExit(IMovementControl exit_control)
        {
            return true;
        }
        protected override void ExitControl()
        {
            _pauseTime = 0f;
        }

        public override Vector2 GetCurrentVelocity()
        {
            return _direction * ControlData.Force;
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

            TryTransition();
        }

        // ##### TRANSITION ##### \\

        public MovementState TransitionState => MovementState.FALLING;

        private void TryTransition()
        {
            if (Time.time - ControlArgs.StartTime + _pauseTime < ControlData.Duration)
                return;

            OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new(Controller)); // FALLING
        }

        // ##### PRE JUMP CHARGE INPUT ##### \\

        private void StartJumpCharge(Vector2 position, float min_hold_duration)
        {
            if (!LevelController.Started)
                return;

            if (!Controller.GetControl<ChargedJumpControl>().CanStart())
                return;

            if (OnInputDetected != null)
                OnInputDetected(this, new FloatControlArgs(new(Controller), MovementDirection.Down));
            ChargingJump = true;
        }

        private void CancelJumpCharge()
        {
            ChargingJump = false;
        }
    }
}