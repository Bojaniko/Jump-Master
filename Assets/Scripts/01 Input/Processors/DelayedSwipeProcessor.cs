using System.Collections;
using System.Collections.Generic;

using Studio28.Utility;

using JumpMaster.LevelTrackers;

using UnityEngine;
using UnityEngine.InputSystem;

namespace JumpMaster.Controls
{
    public class DelayedSwipeProcessor : IInputProcessor
    {
        public event InputStateEventHandler OnStateChanged;
        public event InputPerformedEventHandler OnInputPerformed;

        public InputProcessorDataSO Data => _data;
        private readonly SwipeProcessorDataSO _data;

        private readonly InputAction _positionAction;

        private readonly StateMachine<InputProcessState> _stateController;
        public InputProcessState CurrentState => _stateController.CurrentState;

        public DelayedSwipeProcessor(SwipeProcessorDataSO data, InputAction position_action)
        {
            _data = data;
            _positionAction = position_action;

            _stateController = new("Delayed Swipe Input", InputProcessState.WAITING);
            _stateController.OnStateChanged += (InputProcessState state) => { OnStateChanged?.Invoke(this, state); };
        }

        private TimeRecord _delayTimer;
        private TimeRecord _startTimer;

        private SwipeInput _sInput;

        public void CancelInputProcess()
        {
            CancelDelayTimer();
            CancelStartTimer();
            _stateController.SetState(InputProcessState.CANCELED);
        }
        private void CancelDelayTimer()
        {
            if (_delayTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_delayTimer);
                _delayTimer = null;
            }
        }
        private void CancelStartTimer()
        {
            if (_startTimer != null)
            {
                TimeTracker.Instance.CancelTimeTracking(_startTimer);
                _startTimer = null;
            }
        }

        public void StartInputProcess()
        {
            _sInput = new();
            _sInput.Delay = true;
            _sInput.StartPosition = _positionAction.ReadValue<Vector2>();
            _delayTimer = TimeTracker.Instance.StartTimeTracking(DelayTimerEnded, _data.MaxTime);
        }
        private void DelayTimerEnded()
        {
            _delayTimer = null;
            if (_sInput.StartPosition != _positionAction.ReadValue<Vector2>())
                _stateController.SetState(InputProcessState.CANCELED);
            else
                _stateController.SetState(InputProcessState.STARTED);
        }

        public void StopInputProcess()
        {
            CancelDelayTimer();
            CancelStartTimer();

            _sInput.EndTime = Time.time;
            _sInput.EndPosition = _positionAction.ReadValue<Vector2>();
            Swipe swipe = SwipeInputProcessor.ProcessSwipe(_sInput, _data.MaxTime, _data.MinDistance);
            if (swipe.IsValid())
                OnInputPerformed?.Invoke(this, new SwipePerformedEventArgs(swipe, _sInput.StartTime, _sInput.EndTime));
            _stateController.SetState(InputProcessState.WAITING);
        }

        public void UpdateInputProcess()
        {
            if (_sInput.StartPosition == _positionAction.ReadValue<Vector2>())
                return;
            if (_startTimer != null)
                return;
            _sInput.StartTime = Time.time;
            _startTimer = TimeTracker.Instance.StartTimeTracking(CancelInputProcess, _data.MaxTime);
        }
    }
}