using UnityEngine;
using UnityEngine.InputSystem;

using Studio28.Utility;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Controls
{
    public class TapProcessor : IInputProcessor
    {
        public InputProcessorDataSO Data => _data;
        private readonly TapProcessorDataSO _data;

        public InputAction PositionAction => _positionAction;
        private readonly InputAction _positionAction;

        public InputProcessState CurrentState => _stateController.CurrentState;
        private readonly StateMachine<InputProcessState> _stateController;

        public event InputPerformedEventHandler OnInputPerformed;
        public event InputStateEventHandler OnStateChanged;

        public TapProcessor(TapProcessorDataSO data, InputAction position_action)
        {
            _data = data;
            _positionAction = position_action;

            _stateController = new("Tap Processor", InputProcessState.WAITING);
            _stateController.OnStateChanged += (InputProcessState state) => { OnStateChanged?.Invoke(this, state); };
        }

        private float _startTime;
        private Vector2 _startPosition;

        private TimeRecord _tapTimer;

        public void UpdateInputProcess() { /* Is not needed for tap processing. */ }
        public void StartInputProcess()
        {
            _startTime = Time.time;
            _startPosition = _positionAction.ReadValue<Vector2>();
            _stateController.SetState(InputProcessState.STARTED);
            _tapTimer = TimeTracker.Instance.StartTimeTracking(CancelInputProcess, _data.MaxDuration);
        }
        public void StopInputProcess()
        {
            if (_stateController.CurrentState.Equals(InputProcessState.STARTED) &&
                _startPosition == _positionAction.ReadValue<Vector2>())
                OnInputPerformed?.Invoke(this, new InputPerformedEventArgs(_startTime, Time.time));
            _stateController.SetState(InputProcessState.WAITING);
            if (_tapTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_tapTimer);
                _tapTimer = null;
            }
        }
        public void CancelInputProcess()
        {
            if (_tapTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_tapTimer);
                _tapTimer = null;
            }
            _stateController.SetState(InputProcessState.CANCELED);
        }
    }
}