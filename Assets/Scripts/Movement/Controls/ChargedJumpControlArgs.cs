namespace JumpMaster.Movement
{
    public class ChargedJumpControlArgs : MovementControlArgs
    {
        public readonly MovementDirection Direction;

        public ChargedJumpControlArgs(MovementControlArgs args, MovementDirection direction) : base(args)
        {
            Direction = direction;
        }
    }
}