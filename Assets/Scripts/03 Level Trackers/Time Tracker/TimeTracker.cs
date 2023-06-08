using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Core;

namespace JumpMaster.LevelTrackers
{
    public delegate void TimeRecordCallback();

    [DisallowMultipleComponent]
    public class TimeTracker : LevelController
    {
        public static TimeTracker Instance
        {
            get => s_instance;
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                {
                    Destroy(value);
                    Debug.LogError("You can have only one time tracker in your scene!");
                }
            }
        }
        private static TimeTracker s_instance;

        protected override void Initialize()
        {
            s_instance = this;

            _activeRecords = new();
            LevelManager.OnResume += Resume;
            LevelManager.OnRestart += Restart;
        }
        private void Restart()
        {
            _activeRecords.Clear();
        }
        private void Resume()
        {
            if (_activeRecords.Count == 0)
                return;
            foreach (TimeRecord record in _activeRecords)
                record.ProlongTime(LevelManager.LastPauseDuration);
        }
        private void Update()
        {
            if (LevelManager.Paused || LevelManager.Ended || !LevelManager.Started)
                return;
            if (_activeRecords.Count == 0)
                return;
            for (int r = 0; r < _activeRecords.Count; r++)
            {
                if (Time.time - _activeRecords[r].StartTime > _activeRecords[r].Duration)
                {
                    _activeRecords[r].Callback.Invoke();
                    _activeRecords.Remove(_activeRecords[r]);
                }
            }
        }

        private List<TimeRecord> _activeRecords;
        public TimeRecord StartTimeTracking(TimeRecordCallback callback, float duration, float start_time)
        {
            TimeRecord record = new(callback, duration);
            StartTimeTracking(record);
            return record;
        }
        public TimeRecord StartTimeTracking(TimeRecordCallback callback, float duration)
        {
            TimeRecord record = new(callback, duration);
            StartTimeTracking(record);
            return record;
        }
        public void StartTimeTracking(TimeRecord record)
        {
            if (Time.time - record.StartTime > record.Duration)
                return;
            _activeRecords.Add(record);
        }
        public void CancelTimeTracking(TimeRecord record)
        {
            if (_activeRecords.Contains(record))
                _activeRecords.Remove(record);
        }
    }
}
