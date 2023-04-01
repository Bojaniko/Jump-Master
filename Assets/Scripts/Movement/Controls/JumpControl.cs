using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class JumpControl : MovementControl<JumpControlDataSO>, ITransitionable, IInputableControl
    {
        private Vector3 _topVelocity;
        private float _chainPenaltyTime;

        public int Chain { get; private set; }

        public event ControlInputEventHandler OnInputDetected;

        public delegate void JumpControlEventHander();
        public event JumpControlEventHander OnChainUpdate;
        public event TransitionableControlEventHandler OnTransitionable;

        public JumpControl(MovementController controller, JumpControlDataSO data) : base(controller, data)
        {
            Chain = 0;
            _chainPenaltyTime = 0f;

            Controller.OnActiveControlChange += OnChargedJump;

            if (InputController.Instance != null)
                InputController.Instance.OnTap += JumpInput;
        }
        public override MovementState ActiveState { get { return MovementState.JUMPING; } }

        protected override bool CanStartControl()
        {
            if (Controller.ActiveControl.ActiveState.Equals(MovementState.HANGING))
                return false;

            if (Chain >= ControlData.MaxChain) return false;

            return true;
        }

        public override bool CanExit()
        {
            if (Controller.ActiveControl.ActiveState.Equals(MovementState.JUMPING))
            {
                if ((Controller.gameObject.transform.position.y - ControlArgs.StartPosition.y) / ControlData.Height < ControlData.MinChainHeight)
                    return false;
            }
            return true;
        }

        protected override void StartControl()
        {
            Controller.ControlledRigidbody.useGravity = false;

            _topVelocity = GetVectorDirection(Controller.PreviousControl) * ControlData.Force;

            if (Controller.PreviousControl.ActiveState.Equals(MovementState.DASHING))
                UpdateDirection(new(ControlArgs.Direction.Vertical, Controller.PreviousControl.ControlArgs.Direction.Horizontal));

            PerformChain();

            if (!LevelController.Started)
                LevelController.StartLevel();
        }

        private Vector3 GetVectorDirection(IMovementControl previous_control)
        {
            Vector3 vector_direction = Vector3.up;
            if (previous_control.ActiveState.Equals(MovementState.DASHING))
                vector_direction += Vector3.right * previous_control.ControlArgs.Direction.Horizontal * ControlData.CrossChainVelocityPercentage;
            return vector_direction;
        }

        protected override void ExitControl() { }

        public override Vector3 GetCurrentVelocity()
        {
            float heightPercentage = (Controller.transform.position.y - ControlArgs.StartPosition.y) / ControlData.Height;
            return Vector3.Lerp(_topVelocity, Vector3.zero, heightPercentage);
        }

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

            if (Mathf.Abs(Controller.ControlledRigidbody.velocity.y) > ControlData.EndForce)
                return;

            if (OnTransitionable != null)
            {
                MovementControlArgs start_args = new(Controller.ControlledRigidbody, ControlArgs.Direction, 1f);
                OnTransitionable(Controller.GetControlByState(TransitionState), start_args);
            }
        }

        public override void Resume()
        {
            _chainPenaltyTime += LevelController.LastPauseDuration;
        }
        public override void Pause() { }

        public MovementState TransitionState
        {
            get
            {
                if (ControlArgs.Direction.Horizontal != 0)
                    return MovementState.FALLING;
                return MovementState.FLOATING;
            }
        }

        // Counts the chain if the previous control was a jump.
        private void PerformChain()
        {
            Chain++;
            _chainPenaltyTime = Time.time;

            if (OnChainUpdate != null)
                OnChainUpdate();
        }

        // Check if the player has tapped the screen as a jump input.
        private void JumpInput()
        {
            if (OnInputDetected != null)
                OnInputDetected(this, new(Controller.ControlledRigidbody, MovementDirection.Up));
        }

        // Check if the player has performed a charged jump to count it as a jump.
        private void OnChargedJump(IMovementControl control)
        {
            if (!control.ActiveState.Equals(MovementState.JUMP_CHARGING))
                return;
            PerformChain();
        }
    }
}