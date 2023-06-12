namespace JumpMaster.LevelTrackers
{
    public class TimeRecord
    {
        public bool Invoked => _invoked;
        private bool _invoked;

        private readonly TimeRecordCallback _callback;

        public float StartTime => _startTime;
        private float _startTime;

        public float Duration => _duration;
        private readonly float _duration;

        public TimeRecord(TimeRecordCallback callback, float duration)
        {
            _invoked = false;
            _callback = callback;
            _duration = duration;
            _startTime = UnityEngine.Time.time;
        }

        public TimeRecord(TimeRecordCallback callback, float duration, float start_time)
        {
            _invoked = false;
            _callback = callback;
            _duration = duration;
            _startTime = start_time;
        }

        public void InvokeCallback()
        {
            if (_invoked)
                return;
            _callback.Invoke();
            _invoked = true;
        }
        public void CancelCallback() =>
            _invoked = true;

        public void ProlongTime(float amount) => _startTime += amount;
    }
}