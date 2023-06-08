namespace JumpMaster.Controls
{
    public class InputPerformedEventArgs : IInputPerformedEventArgs
    {
        public float StartTime => _startTime;
        private readonly float _startTime;

        public float PerformTime => _performTime;
        private readonly float _performTime;

        public InputPerformedEventArgs(float start_time, float perform_time)
        {
            _startTime = start_time;
            _performTime = perform_time;
        }
    }
}