using JumpMaster.LevelControllers;
using JumpMaster.Controls;

using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class JumpControl : MovementControl<JumpControlDataSO, MovementControlArgs>, IPrimaryControl, ITransitionable, IInputableControl, IChainable
    {
        private float _chainPenaltyTime;
        private float _heightPercentage;

        private Vector2 _topVelocity;

        public int Chain { get; private set; }

        public event ChainEventHandler OnChain;
        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        public JumpControl(MovementController controller, JumpControlDataSO data) : base(controller, data)
        {
            Chain = 0;
            _chainPenaltyTime = 0f;

            Controller.OnActiveControlChange += OnChargedJump;

            LevelController.OnRestart += Restart;

            if (InputController.Instance != null)
                InputController.Instance.OnTap += JumpInput;
        }
        public MovementState TransitionState
        {
            get
            {
                if (_topVelocity.x != 0f)
                    return MovementState.FALLING;
                return MovementState.FLOATING;
            }
        }
        public override MovementState ActiveState { get { return MovementState.JUMPING; } }

        protected override void OnMovementUpdate()
        {
            TryRestartChain();

            if (!Started)
                return;

            _heightPercentage = (Controller.transform.position.y - ControlArgs.StartPosition.y) / ControlData.Height;

            if (Mathf.Abs(Controller.ControlledRigidbody.velocity.y) > 0.5f)
                return;

            MovementControlArgs start_args = new(Controller);

            if (TransitionState.Equals(MovementState.FLOATING))
                OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new FloatControlArgs(start_args, MovementDirection.Up));
            else
                OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), start_args); // FALLING
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

            if (!LevelController.Started)
                LevelController.StartLevel();
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

        public override void Resume()
        {
            _chainPenaltyTime += LevelController.LastPauseDuration;
        }
        public override void Pause() { }

        private void Restart()
        {
            Chain = 0;
            _chainPenaltyTime = 0f;
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

            if (previous_primary_control is IPrimaryControl)
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
    }
}