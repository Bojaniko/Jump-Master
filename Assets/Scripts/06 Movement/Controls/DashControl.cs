using JumpMaster.Core;
using JumpMaster.Controls;
using JumpMaster.LevelTrackers;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class DashControl : MovementControl<DashControlDataSO, DashControlArgs>, IPrimaryControl, ITransitionable, IInputableControl, IDirectional, IChainable
    {
        private const float _transitionDistancePercentage = 0.9f;
        
        private float _distancePercentage;

        private Vector2 _topVelocity;

        public MovementDirection Direction
        {
            get
            {
                if (_controlArgs == null)
                    return MovementDirection.Zero;
                return _controlArgs.Direction;
            }
        }

        public event ChainEventHandler OnChain;
        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        public DashControl(MovementController controller, DashControlDataSO data) : base(controller, data)
        {
            _controlArgs = new DashControlArgs(new(controller), MovementDirection.Zero);

            _distancePercentage = 0f;

            LevelManager.OnRestart += Restart;

            InputController.Instance.RegisterInputPerformedListener<SwipeProcessor>(DashInput, this);
            InputController.Instance.RegisterInputPerformedListener<DelayedSwipeProcessor>(DelayedDashInput, this);
        }
        public override MovementState ActiveState { get { return MovementState.DASHING; } }
        public MovementState TransitionState => MovementState.FALLING;

        protected override void OnMovementUpdate()
        {
            if (!Started)
                return;

            _distancePercentage = Mathf.Abs(ControlArgs.StartPosition.x - Controller.transform.position.x) / ControlData.MaxDistance;//ControlData.Distance;

            bool transitionableDistanceReached = (_distancePercentage >= _transitionDistancePercentage);
            if (!transitionableDistanceReached)
                return;
            OnTransitionable?.Invoke(this, Controller.GetControlByState(TransitionState), new(Controller)); // FALLING
        }

        // ##### CONTROL INSTRUCTIONS ##### \\

        protected override bool CanStartControl()
        {
            if (!LevelManager.Started)
                return false;

            if (Chain >= ControlData.MaxChain)
                return false;

            return true;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.gravityScale = 0f;

            _distancePercentage = 0f;

            _topVelocity = GetTopVelocity(Controller.PreviousControl.ActiveState, _controlArgs.Direction.Horizontal);

            PerformChain();
        }

        public override bool CanExit(IMovementControl exit_control)
        {
            if (exit_control.ActiveState.Equals(MovementState.DASHING) && _distancePercentage < ControlData.MinChainDistance)
                return false;
            return true;
        }
        protected override void ExitControl()
        {
            _distancePercentage = 0f;
        }

        public override void Pause() { }
        public override void Resume() { }

        private void Restart()
        {
            Chain = 0;
            _chainPenaltyTimer = null;
            _distancePercentage = 0f;
        }

        // ##### PHYSICS ##### \\

        public override Vector2 GetCurrentVelocity()
        {
            float horizontalVelocity = Mathf.Lerp(_topVelocity.x, 0, 1f - ControlData.HorizontalVelocityFalloff.Evaluate(_distancePercentage));
            float verticalVelocity = 0;

            //if (Controller.ControlledRigidbody.transform.position.y - _controlArgs.StartPosition.y < ControlData.MaxCrossChainVerticalDistance)
                verticalVelocity = Mathf.Lerp(_topVelocity.y, Physics2D.gravity.y, ControlData.GravityFalloff.Evaluate(_distancePercentage));

            return new(horizontalVelocity, verticalVelocity);
        }

        private Vector2 GetTopVelocity(MovementState transition_state, int horizontal_direction)
        {
            Vector2 velocity = Vector2.right * horizontal_direction * ControlData.Force;

            if (transition_state.Equals(MovementState.JUMPING)
                || transition_state.Equals(MovementState.FLOATING)
                || transition_state.Equals(MovementState.HANGING))
            {
                velocity += Vector2.up * ControlData.CrossChainVerticalForce;
            }
            else velocity += Vector2.up * ControlData.VerticalForce;
            return velocity;
        }

        // ##### CHAIN ##### \\

        public int Chain { get; private set; }

        private TimeRecord _chainPenaltyTimer;

        private void PerformChain()
        {
            Chain++;
            OnChain?.Invoke(Chain, ControlData.MaxChain);

            if (_chainPenaltyTimer != null)
                TimeTracker.Instance.CancelTimeTracking(_chainPenaltyTimer);
            _chainPenaltyTimer = TimeTracker.Instance.StartTimeTracking(RestartChain, ControlData.ChainPenaltyDuration);
        }

        private void RestartChain()
        {
            _chainPenaltyTimer = null;
            Chain = 0;
            OnChain?.Invoke(Chain, ControlData.MaxChain);
        }

        // ##### INPUT ##### \\

        private void DashInput(IInputPerformedEventArgs args)
        {
            Swipe swipe = ((SwipePerformedEventArgs)args).SwipeData;

            int dir = 0;
            if (swipe.Direction.Equals(SwipeDirection.LEFT)) dir = -1;
            if (swipe.Direction.Equals(SwipeDirection.RIGHT)) dir = 1;

            if (dir == 0)
                return;

            if (Controller.ActiveControl.ActiveState.Equals(MovementState.HANGING))
            {
                HangControlArgs hangArgs = (HangControlArgs)Controller.ActiveControl.ControlArgs;
                if (hangArgs.Direction.Horizontal != dir)
                    return;
            }

            MovementDirection directionMove = new(0, dir);
            OnInputDetected?.Invoke(this, new DashControlArgs(new(Controller), directionMove));
        }

        private void DelayedDashInput(IInputPerformedEventArgs args)
        {
            Swipe swipe = ((SwipePerformedEventArgs)args).SwipeData;

            int dir = 0;
            if (swipe.Direction.Equals(SwipeDirection.LEFT)) dir = -1;
            if (swipe.Direction.Equals(SwipeDirection.RIGHT)) dir = 1;

            if (dir == 0)
                return;

            if (!Controller.PreviousPrimaryControl.ActiveState.Equals(MovementState.LEVITATING))
                return;

            MovementDirection directionMove = new(0, dir);
            OnInputDetected?.Invoke(this, new DashControlArgs(new(Controller), directionMove));
        }
    }
}