using UnityEngine;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Movement
{
    public class FloatControl : MovementControl<FloatControlDataSO, FloatControlArgs>, IDirectional, ITransitionable
    {
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

        public event TransitionableControlEventHandler OnTransitionable;

        public FloatControl(MovementController controller, FloatControlDataSO data) : base(controller, data) { }
        public override MovementState ActiveState { get { return MovementState.FLOATING; } }

        protected override bool CanStartControl() => true;
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.gravityScale = 0f;
            _direction = Vector2.up * _controlArgs.Direction.Vertical * ControlArgs.Strength;
            _transitionTimer = TimeTracker.Instance.StartTimeTracking(Transition, ControlData.Duration);
        }

        public override bool CanExit(IMovementControl exit_control) => true;
        protected override void ExitControl()
        {
            if (_transitionTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_transitionTimer);
                _transitionTimer = null;
            }
        }

        public override Vector2 GetCurrentVelocity()
        {
            return _direction * ControlData.Force;
        }

        public override void Pause() { }
        public override void Resume() { }
        protected override void OnMovementUpdate() { }

        public MovementState TransitionState => MovementState.FALLING;

        private TimeRecord _transitionTimer;

        private void Transition()
        {
            _transitionTimer = null;
            OnTransitionable?.Invoke(this, Controller.GetControlByState(TransitionState), new(Controller));
        }
    }
}