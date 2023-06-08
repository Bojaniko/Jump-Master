namespace JumpMaster.Controls
{
    public enum SwipeDirection { UP, DOWN, LEFT, RIGHT, NONE }

    public readonly struct Swipe
    {
        public readonly bool Delayed;
        public readonly SwipeDirection Direction;

        public Swipe(SwipeDirection direction)
        {
            Delayed = false;
            Direction = direction;
        }

        public Swipe(SwipeDirection direction, bool delayed)
        {
            Delayed = delayed;
            Direction = direction;
        }

        public bool IsValid() => (!Direction.Equals(SwipeDirection.NONE));

        public static Swipe Invalid => new(SwipeDirection.NONE);
    }
}