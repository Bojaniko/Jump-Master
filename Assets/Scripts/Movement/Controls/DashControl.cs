using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class DashControl : MovementControl<DashControlDataSO>, ITransitionable, IInputableControl
    {
        private float _chainPenaltyTime;
        private Vector3 _vectorDirection;

        public float DistancePercentage { get; private set; }
        public int Chain { get; private set; }

        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        public delegate void DashControlEventHander();
        public event DashControlEventHander OnChainUpdate;

        public DashControl(MovementController controller, DashControlDataSO data) : base(controller, data)
        {
            if (SwipeDetector.Instance != null)
                SwipeDetector.Instance.OnSwipeDetected += DashInput;

            DistancePercentage = 0f;
        }
        public override MovementState ActiveState { get { return MovementState.DASHING; } }

        protected override void OnMovementUpdate()
        {
            if (Chain > 0 && Time.time - _chainPenaltyTime > ControlData.ChainPenaltyDuration)
            {
                Chain = 0;
                if (OnChainUpdate != null)
                    OnChainUpdate();
            }

            if (!Started)
                return;

            DistancePercentage = Mathf.Abs(ControlArgs.StartPosition.x - Controller.transform.position.x) / ControlData.Distance;

            if (DistancePercentage < ControlData.EndDistancePercentage)
                return;

            if (OnTransitionable != null)
            {
                MovementControlArgs start_args = new(Controller.ControlledRigidbody, ControlArgs.Direction, 1f);
                OnTransitionable(Controller.GetControlByState(TransitionState), start_args);
            }
        }

        public override Vector3 GetCurrentVelocity()
        {
            if (DistancePercentage < ControlData.MinChainDistance)
                return Vector3.Lerp(_vectorDirection * ControlData.Force, Vector3.zero, DistancePercentage);
            else
                return Vector3.Lerp(_vectorDirection * ControlData.Force, Physics.gravity, DistancePercentage);
        }

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
            Controller.ControlledRigidbody.useGravity = false;

            Chain++;
            _chainPenaltyTime = ControlArgs.StartTime;

            if (OnChainUpdate != null)
                OnChainUpdate();

            _vectorDirection = GetVectorDirection(Controller.PreviousControl.ActiveState);
        }

        public override bool CanExit()
        {
            if (!Started)
                return false;

            if (DistancePercentage < ControlData.MinChainDistance)
                return false;

            return true;
        }
        protected override void ExitControl()
        {
            DistancePercentage = 0f;
        }

        public MovementState TransitionState
        {
            get
            {
                return MovementState.FALLING;
            }
        }

        public override void Pause() { }
        public override void Resume()
        {
            _chainPenaltyTime += LevelController.LastPauseDuration;
        }

        // Detect swipe input for dashing.
        private void DashInput(SwipeDirection direction)
        {
            int dir = 0;
            if (direction.Equals(SwipeDirection.LEFT)) dir = -1;
            if (direction.Equals(SwipeDirection.RIGHT)) dir = 1;

            if (dir == 0)
                return;

            if (Controller.ActiveControl.ActiveState.Equals(MovementState.HANGING))
            {
                if (Controller.ActiveControl.ControlArgs.Direction.Horizontal != dir)
                    return;
            }

            MovementDirection direction_move = new(0, dir);

            OnInputDetected(this, new(Controller.ControlledRigidbody, direction_move));
        }

        // Get the direction the player should move at.
        private Vector3 GetVectorDirection(MovementState transition_state)
        {
            Vector3 vector_direction = Vector3.right * ControlArgs.Direction.Horizontal;

            if (transition_state.Equals(MovementState.JUMPING)
                || transition_state.Equals(MovementState.FLOATING)
                || transition_state.Equals(MovementState.HANGING))
            {
                vector_direction += Vector3.up * ControlData.CrossChainVelocity;
                UpdateDirection(new(1, ControlArgs.Direction.Horizontal));
            }
            return vector_direction;
        }
    }
}