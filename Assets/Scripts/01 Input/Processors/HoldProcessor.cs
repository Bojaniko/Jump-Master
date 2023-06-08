using UnityEngine;
using UnityEngine.InputSystem;

using Studio28.Utility;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Controls
{
    public class HoldProcessor : IInputProcessor
    {
        public event InputPerformedEventHandler OnInputPerformed;
        public event InputStateEventHandler OnStateChanged;

        public InputProcessorDataSO Data => _data;
        private readonly HoldProcessorDataSO _data;

        private readonly InputAction _positionAction;

        public InputProcessState CurrentState => _stateController.CurrentState;
        private readonly StateMachine<InputProcessState> _stateController;

        public HoldProcessor(HoldProcessorDataSO data, InputAction position_action)
        {
            _data = data;
            _positionAction = position_action;

            _stateController = new("Hold Processor", InputProcessState.WAITING);
            _stateController.OnStateChanged += (InputProcessState state) => { OnStateChanged?.Invoke(this, state); };
        }

        private float _startTime;
        private Vector2 _startPosition;

        private TimeRecord _holdTimer;

        public void CancelInputProcess()
        {
            CancelHoldTimer();
            _stateController.SetState(InputProcessState.CANCELED);
        }
        private void CancelHoldTimer()
        {
            if (_holdTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_holdTimer);
                _holdTimer = null;
            }
        }

        public void StartInputProcess()
        {
            _startTime = Time.time;
            _startPosition = _positionAction.ReadValue<Vector2>();
            _holdTimer = TimeTracker.Instance.StartTimeTracking(HoldTimerEnded, _data.MinDuration);
            _stateController.SetState(InputProcessState.STARTED);
        }
        private void HoldTimerEnded() =>
            OnInputPerformed?.Invoke(this, new InputPerformedEventArgs(_startTime, Time.time));

        public void StopInputProcess()
        {
            CancelHoldTimer();
            _stateController.SetState(InputProcessState.WAITING);
        }

        public void UpdateInputProcess()
        {
            if (_startPosition != _positionAction.ReadValue<Vector2>())
                CancelInputProcess();
        }
    }
}