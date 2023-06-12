namespace JumpMaster.Movement
{
    public class DashControlArgs : MovementControlArgs
    {
        public readonly MovementDirection Direction;

        public readonly float TargetDistance;

        public DashControlArgs(MovementControlArgs args, MovementDirection direction, float target_distance) : base(args)
        {
            Direction = direction;
            TargetDistance = target_distance;
        }
    }
}