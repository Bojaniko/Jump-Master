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
            InputController.Instance.RegisterInputPerformedListener<HoldProcessor>(EndLevitation, this);
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
        protected override void ExitControl()
        {
            if (_transitionTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_transitionTimer);
                _transitionTimer = null;
            }
            TimeTracker.Instance.StartTimeTracking(EndCooldown, ControlData.Cooldown);
        }

        public override void Pause() { }
        public override void Resume() { }
        protected override void OnMovementUpdate() { }

        private bool _cooldownEnded;
        private void EndCooldown() => _cooldownEnded = true;

        // ##### TRANSITION ##### \\

        public MovementState TransitionState => MovementState.FALLING;
        private TimeRecord _startTimer;
        private TimeRecord _transitionTimer;

        private void Transition()
        {
            _transitionTimer = null;
            OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new(Controller));
        }

        private void HoldInput(InputProcessState state, InputProcessorDataSO processor_data)
        {
            if (state.Equals(InputProcessState.STARTED))
            {
                HoldProcessorDataSO data = (HoldProcessorDataSO)processor_data;
                _startTimer = TimeTracker.Instance.StartTimeTracking(StartLevitation, data.MinDuration);
            }
            else if (state.Equals(InputProcessState.CANCELED))
            {
                if (_startTimer != null)
                {
                    TimeTracker.Instance.CancelTimeTracking(_startTimer);
                    _startTimer = null;
                }
            }
        }
        private void StartLevitation()
        {
            _startTimer = null;
            OnInputDetected?.Invoke(this, new MovementControlArgs(Controller));
        }

        private void EndLevitation(IInputPerformedEventArgs args)
        {
            if (!Started)
                return;
            if (_transitionTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_transitionTimer);
                _transitionTimer = null;
            }
            OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new(Controller));
        }
    }
}