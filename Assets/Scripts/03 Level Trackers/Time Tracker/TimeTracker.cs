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

            _records = new();
            LevelManager.OnResume += Resume;
            LevelManager.OnRestart += Restart;
        }
        private void Restart()
        {
            _records.Clear();
        }
        private void Resume()
        {
            if (_records.Count == 0)
                return;
            foreach (TimeRecord record in _records)
                record.ProlongTime(LevelManager.LastPauseDuration);
        }
        private void Update()
        {
            if (LevelManager.Paused || LevelManager.Ended || !LevelManager.Started)
                return;
            if (_records.Count == 0)
                return;
            ProcessRecords();
        }

        private void ProcessRecords()
        {
            for (int r = 0; r < _records.Count; r++)
            {
                if (_records.Peek().Invoked)
                {
                    _records.Dequeue();
                    continue;
                }
                if (Time.time - _records.Peek().StartTime > _records.Peek().Duration)
                {
                    _records.Peek().InvokeCallback();
                    _records.Dequeue();
                    continue;
                }
                _records.Enqueue(_records.Dequeue());
            }
        }

        private Queue<TimeRecord> _records;
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
            _records.Enqueue(record);
        }
        public void CancelTimeTracking(TimeRecord record) =>
            record.CancelCallback();
    }
}