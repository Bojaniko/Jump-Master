using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class JumpControl : MovementControl<JumpControlDataSO, MovementControlArgs>, ITransitionable, IInputableControl, IChainable
    {
        private Vector2 _maxVelocity;
        private float _chainPenaltyTime;

        public int Chain { get; private set; }

        public event ChainEventHandler OnChain;
        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        public override Vector3 GetCurrentVelocity()
        {
            float heightPercentage = (Controller.transform.position.y - ControlArgs.StartPosition.y) / ControlData.Height;
            return Vector2.Lerp(_maxVelocity, Vector2.zero, heightPercentage);
        }

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
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.useGravity = false;

            _maxVelocity = GetVectorDirection(Controller.PreviousControl) * ControlData.Force;

            PerformChain();

            if (!LevelController.Started)
                LevelController.StartLevel();
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
        protected override void ExitControl() { }

        protected override void OnMovementUpdate()
        {
            TryRestartChain();

            if (!Started)
                return;

            if (Mathf.Abs(Controller.ControlledRigidbody.velocity.y) > ControlData.EndForce)
                return;

            MovementControlArgs start_args = new(Controller);

            if (TransitionState.Equals(MovementState.FLOATING))
                OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new FloatControlArgs(start_args, MovementDirection.Up));
            else
                OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new FallControlArgs(start_args));
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
                if (_maxVelocity.x != 0f)
                    return MovementState.FALLING;
                return MovementState.FLOATING;
            }
        }

        // ##### INPUT ##### \\

        private void JumpInput()
        {
            OnInputDetected?.Invoke(this, new MovementControlArgs(Controller));
        }

        private void OnChargedJump(IMovementControl control)
        {
            if (!control.ActiveState.Equals(MovementState.JUMP_CHARGING))
                return;
            PerformChain();
        }

        // ##### CHAIN ##### \\

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

        // ##### DETERMINE MOVEMENT DIRECTION ##### \\

        private Vector2 GetVectorDirection(IMovementControl previous_control)
        {
            Vector2 vector_direction = Vector2.up;

            if (previous_control.ActiveState.Equals(MovementState.DASHING))
            {
                DashControlArgs dashArgs = (DashControlArgs)previous_control.ControlArgs;
                vector_direction += Vector2.right * dashArgs.Direction.Horizontal * ControlData.CrossChainVelocityPercentage;
            }

            return vector_direction;
        }
    }
}