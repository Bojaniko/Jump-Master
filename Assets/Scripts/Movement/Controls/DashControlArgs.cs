namespace JumpMaster.Movement
{
    public class DashControlArgs : MovementControlArgs
    {
        public readonly MovementDirection Direction;

        public DashControlArgs(MovementControlArgs args, MovementDirection direction) : base(args)
        {
            Direction = direction;
        }
    }
}