using UnityEngine;

using JumpMaster.Core;
using JumpMaster.LevelTrackers;
using JumpMaster.Controls;

namespace JumpMaster.Movement
{
    public class LevitationControl : MovementControl<LevitationControlDataSO, MovementControlArgs>, IInputableControl, ITransitionable
    {
        public LevitationControl(MovementController controller, LevitationControlDataSO data) : base(controller, data)
        {
            _cooldownEnded = true;
            InputController.Instance.RegisterInputStateChangeListener<HoldProcessor>(HoldInput, this);
            InputController.Instance.RegisterInputPerformedListener<HoldProcessor>(LevitationInput, this);
        }

        public override MovementState ActiveState => MovementState.LEVITATING;

        public event ControlInputEventHandler OnInputDetected;
        public event TransitionableControlEventHandler OnTransitionable;

        private Vector2 _direction;

        public override Vector2 GetCurrentVelocity()
        {
            return _direction * ControlData.GravityForce;
        }

        protected override bool CanStartControl()
        {
            if (!LevelManager.Started)
                return false;
            if (Controller.ActiveControl.ActiveState.Equals(MovementState.HANGING))
                return false;
            return (_cooldownEnded);
        }
        protected override void StartControl()
        {
            _cooldownEnded = false;
            _direction = Vector2.up * -1;
            Controller.ControlledRigidbody.gravityScale = 0f;
            _transitionTimer = TimeTracker.Instance.StartTimeTracking(Transition, ControlData.Duration);
        }

        public override bool CanExit(IMovementControl exit_control) => true;
        protected override void ExitControl() =>
            TimeTracker.Instance.StartTimeTracking(EndCooldown, ControlData.Cooldown);

        public override void Pause() { }
        public override void Resume() { }
        protected override void OnMovementUpdate() { }

        private bool _cooldownEnded;
        private void EndCooldown() => _cooldownEnded = true;

        // ##### TRANSITION ##### \\

        // TODO: Fix input processing

        public MovementState TransitionState => MovementState.FALLING;
        private TimeRecord _transitionTimer;

        private void Transition() =>
            OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new(Controller));

        // ##### INPUT ##### \\

        private void HoldInput(InputProcessState state, InputProcessorDataSO processor_data)
        {
            if (state.Equals(InputProcessState.CANCELED))
                Transition();
        }

        private void LevitationInput(IInputPerformedEventArgs args) =>
            OnInputDetected?.Invoke(this, new MovementControlArgs(Controller));
    }
}