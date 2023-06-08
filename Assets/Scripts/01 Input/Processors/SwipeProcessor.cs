using UnityEngine;
using UnityEngine.InputSystem;

using Studio28.Utility;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Controls
{
    public class SwipeProcessor : IInputProcessor
    {
        public event InputPerformedEventHandler OnInputPerformed;
        public event InputStateEventHandler OnStateChanged;

        public InputProcessorDataSO Data => _data;
        private readonly SwipeProcessorDataSO _data;
        public InputAction PositionAction => _positionAction;
        private readonly InputAction _positionAction;

        public InputProcessState CurrentState => _stateController.CurrentState;
        private readonly StateMachine<InputProcessState> _stateController;

        public SwipeProcessor(SwipeProcessorDataSO data, InputAction position_action)
        {
            _data = data;
            _positionAction = position_action;

            _stateController = new("Swipe processor", InputProcessState.WAITING);
            _stateController.OnStateChanged += (InputProcessState state) => { OnStateChanged?.Invoke(this, state); };
        }

        private SwipeInput _sInput;
        private TimeRecord _startTimer;

        private void EndStartInputTimer()
        {
            _startTimer = null;
            _stateController.SetState(InputProcessState.CANCELED);
        }

        public void UpdateInputProcess() { /* Update is not used because it is not needed to process a swipe. */ }
        public void StartInputProcess()
        {
            _sInput = new();
            _sInput.Delay = false;
            _sInput.StartTime = Time.time;
            _sInput.StartPosition = PositionAction.ReadValue<Vector2>();
            _startTimer = TimeTracker.Instance.StartTimeTracking(EndStartInputTimer, _data.MaxTime);
            _stateController.SetState(InputProcessState.STARTED);
        }
        public void StopInputProcess()
        {
            CancelStartTimer();

            _sInput.EndTime = Time.time;
            _sInput.EndPosition = _positionAction.ReadValue<Vector2>();

            Swipe swipe = SwipeInputProcessor.ProcessSwipe(_sInput, _data.MaxTime, _data.MinDistance);
            if (swipe.IsValid())
                OnInputPerformed?.Invoke(this, new SwipePerformedEventArgs(swipe, _sInput.StartTime, _sInput.EndTime));
            _stateController.SetState(InputProcessState.WAITING);
        }

        public void CancelInputProcess()
        {
            CancelStartTimer();
            _stateController.SetState(InputProcessState.CANCELED);
        }
        private void CancelStartTimer()
        {
            if (_startTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_startTimer);
                _startTimer = null;
            }
        }
    }
}