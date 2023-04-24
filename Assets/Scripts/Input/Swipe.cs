namespace JumpMaster.Controls
{
    public readonly struct Swipe
    {
        public readonly float StartTime;
        public readonly float DistanceScreen;
        public readonly SwipeDirection Direction;

        public Swipe(SwipeDirection direction, float distance_screen, float start_time)
        {
            StartTime = start_time;
            DistanceScreen = distance_screen;
            Direction = direction;
        }
    }
}