namespace JumpMaster.Controls
{
    public sealed class SwipePerformedEventArgs : InputPerformedEventArgs
    {
        public Swipe SwipeData => _swipeData;
        private Swipe _swipeData;

        public SwipePerformedEventArgs(Swipe swipe_data, float start_time, float perform_time) : base(start_time, perform_time)
        {
            _swipeData = swipe_data;
        }
    }
}