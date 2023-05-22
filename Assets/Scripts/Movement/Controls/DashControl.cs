using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class DashControl : MovementControl<DashControlDataSO, DashControlArgs>, IPrimaryControl, ITransitionable, IInputableControl, IDirectional, IChainable
    {
        private readonly float _pixelWorld;

        private float _chainPenaltyTime;
        private float _distancePercentage;
        private float _targetDistance;

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
            if (SwipeDetector.Instance != null)
                SwipeDetector.Instance.OnSwipeDetected += DashInput;

            _pixelWorld = Vector2.Distance(Camera.main.ScreenToWorldPoint(Vector3.zero), Camera.main.ScreenToWorldPoint(Vector3.right * 100));

            _controlArgs = new DashControlArgs(new(controller), MovementDirection.Zero, 1f);

            _distancePercentage = 0f;

            LevelController.OnRestart += Restart;
        }
        public override MovementState ActiveState { get { return MovementState.DASHING; } }

        protected override void OnMovementUpdate()
        {
            TryRestartChain();

            if (!Started)
                return;

            _distancePercentage = Mathf.Abs(ControlArgs.StartPosition.x - Controller.transform.position.x) / _targetDistance;//ControlData.Distance;

            TryTransition();
        }

        // ##### CONTROL INSTRUCTIONS ##### \\

        protected override bool CanStartControl()
        {
            if (!LevelController.Started)
                return false;

            if (Chain >= ControlData.MaxChain)
                return false;

            return true;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.gravityScale = 0f;

            _distancePercentage = 0f;

            _targetDistance = Mathf.Clamp(_controlArgs.TargetDistance, ControlData.MinDistance, ControlData.MaxDistance);

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
        public override void Resume()
        {
            _chainPenaltyTime += LevelController.LastPauseDuration;
        }

        private void Restart()
        {
            Chain = 0;
            _chainPenaltyTime = 0f;
            _distancePercentage = 0f;
        }

        // ##### PHYSICS ##### \\

        public override Vector2 GetCurrentVelocity()
        {
            float horizontalVelocity = Mathf.Lerp(_topVelocity.x, 0, 1f - ControlData.HorizontalVelocityFalloff.Evaluate(_distancePercentage));
            float verticalVelocity = 0;

            if (Controller.ControlledRigidbody.transform.position.y - _controlArgs.StartPosition.y < ControlData.MaxCrossChainVerticalDistance)
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
            return velocity;
        }

        // ##### CHAIN ##### \\

        public int Chain { get; private set; }

        private void PerformChain()
        {
            Chain++;
            _chainPenaltyTime = Time.time;

            OnChain?.Invoke(Chain, ControlData.MaxChain);
        }

        private void TryRestartChain()
        {
            if (Chain == 0)
                return;

            if (Time.time - _chainPenaltyTime < ControlData.ChainPenaltyDuration)
                return;

            Chain = 0;
            OnChain?.Invoke(Chain, ControlData.MaxChain);
        }

        // ##### TRANSITION ##### \\

        public MovementState TransitionState => MovementState.FALLING;
        private void TryTransition()
        {
            if (_distancePercentage < ControlData.TransitionDistancePercentage)
                return;

            OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new(Controller)); // FALLING
        }

        // ##### INPUT ##### \\

        private void DashInput(Swipe swipe)
        {
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

            float targetDistance = (swipe.DistanceScreen * _pixelWorld) / 100f;
            MovementDirection directionMove = new(0, dir);
            OnInputDetected?.Invoke(this, new DashControlArgs(new(Controller), directionMove, targetDistance));
        }
    }
}