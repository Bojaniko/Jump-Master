namespace JumpMaster.LevelTrackers
{
    public class TimeRecord
    {
        public TimeRecordCallback Callback => _callback;
        private readonly TimeRecordCallback _callback;

        public float StartTime => _startTime;
        private float _startTime;

        public float Duration => _duration;
        private readonly float _duration;

        public TimeRecord(TimeRecordCallback callback, float duration)
        {
            _callback = callback;
            _duration = duration;
            _startTime = UnityEngine.Time.time;
        }

        public TimeRecord(TimeRecordCallback callback, float duration, float start_time)
        {
            _callback = callback;
            _duration = duration;
            _startTime = start_time;
        }

        public void ProlongTime(float amount) => _startTime += amount;
    }
}