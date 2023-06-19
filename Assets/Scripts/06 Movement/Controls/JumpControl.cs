using JumpMaster.Core;
using JumpMaster.LevelTrackers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class JumpControl : MovementControl<JumpControlDataSO, MovementControlArgs>, IPrimaryControl, ITransitionable, IInputableControl, IChainable
    {
        private float _heightPercentage;
        private const float _transitionHeightPercentage = 0.9f;

        private Vector2 _topVelocity;

        public int Chain { get; private set; }

        public event ChainEventHandler OnChain;
        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        public JumpControl(MovementController controller, JumpControlDataSO data) : base(controller, data)
        {
            Chain = 0;

            LevelManager.OnRestart += Restart;

            InputController.Instance.RegisterInputPerformedListener<TapProcessor>(JumpInput, this);
            InputController.Instance.RegisterInputPerformedListener<DelayedSwipeProcessor>(JumpSwipeInput, this);
        }
        public MovementState TransitionState
        {
            get
            {
                bool hasHorizontalVelocity = (_topVelocity.x != 0f);
                if (hasHorizontalVelocity)
                    return MovementState.FALLING;
                return MovementState.FLOATING;
            }
        }
        public override MovementState ActiveState { get { return MovementState.JUMPING; } }

        protected override void OnMovementUpdate()
        {
            if (!Started)
                return;

            _heightPercentage = (Controller.transform.position.y - ControlArgs.StartPosition.y) / ControlData.Height;

            //if (Mathf.Abs(Controller.ControlledRigidbody.velocity.y) > 0.5f)
            //    return;

            bool transitionHeightReached = (_heightPercentage >= _transitionHeightPercentage);

            if (!transitionHeightReached)
                return;

            MovementControlArgs start_args = new(Controller);

            if (TransitionState.Equals(MovementState.FLOATING))
                OnTransitionable?.Invoke(this, Controller.GetControlByState(TransitionState), new FloatControlArgs(start_args, MovementDirection.Up));
            else
                OnTransitionable?.Invoke(this, Controller.GetControlByState(TransitionState), start_args); // FALLING
        }

        // ##### CONTROL ##### \\

        protected override bool CanStartControl()
        {
            if (Chain >= ControlData.MaxChain) return false;

            return true;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.gravityScale = 0f;

            _heightPercentage = 0f;

            _topVelocity = GetTopVelocity(Controller.PreviousPrimaryControl);

            PerformChain();

            if (!LevelManager.Started)
                LevelManager.StartLevel();
        }

        public override bool CanExit(IMovementControl exit_control)
        {
            if (exit_control.ActiveState.Equals(MovementState.JUMPING))
            {
                if ((Controller.gameObject.transform.position.y - ControlArgs.StartPosition.y) / ControlData.Height < ControlData.MinChainHeight)
                    return false;
            }
            return true;
        }
        protected override void ExitControl() { }

        public override void Resume() { }
        public override void Pause() { }

        private void Restart()
        {
            Chain = 0;
            _chainPenaltyTimer = null;
            _heightPercentage = 0f;
        }

        // ##### PHYSICS ##### \\

        public override Vector2 GetCurrentVelocity()
        {
            return Vector2.Lerp(_topVelocity, Vector2.zero, 1f - ControlData.VelocityFalloff.Evaluate(_heightPercentage));
        }

        private Vector2 GetTopVelocity(IMovementControl previous_primary_control)
        {
            Vector2 velocity = Vector2.up * ControlData.Force;

            if (previous_primary_control is IPrimaryControl &&
                !Controller.PreviousControl.ActiveState.Equals(MovementState.FLOATING) &&
                !previous_primary_control.ActiveState.Equals(MovementState.LEVITATING))
            {
                var directionalControl = previous_primary_control as IDirectional;
                if (directionalControl != null)
                    velocity += Vector2.right * directionalControl.Direction.Horizontal * ControlData.CrossChainHorizontalForce;
            }

            return velocity;
        }

        // ##### CHAIN ##### \\

        private void PerformChain()
        {
            Chain++;
            OnChain?.Invoke(Chain, ControlData.MaxChain);

            if (_chainPenaltyTimer != null)
                TimeTracker.Instance.CancelTimeTracking(_chainPenaltyTimer);
            _chainPenaltyTimer = TimeTracker.Instance.StartTimeTracking(RestartChain, ControlData.ChainPenaltyDuration);
        }

        private TimeRecord _chainPenaltyTimer;
        private void RestartChain()
        {
            _chainPenaltyTimer = null;
            Chain = 0;
            OnChain?.Invoke(Chain, ControlData.MaxChain);
        }

        // ##### INPUT ##### \\

        private void JumpInput(IInputPerformedEventArgs args) =>
            OnInputDetected?.Invoke(this, new MovementControlArgs(Controller));

        private void JumpSwipeInput(IInputPerformedEventArgs args)
        {
            SwipePerformedEventArgs swipeArgs = (SwipePerformedEventArgs)args;
            if (!Controller.PreviousPrimaryControl.ActiveState.Equals(MovementState.LEVITATING))
                return;
            if (!swipeArgs.SwipeData.Direction.Equals(SwipeDirection.UP))
                return;
            OnInputDetected?.Invoke(this, new MovementControlArgs(Controller));
        }
    }
}