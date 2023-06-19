namespace JumpMaster.Movement
{
    public class DashControlArgs : MovementControlArgs
    {
        public MovementDirection Direction => _direction;
        private readonly MovementDirection _direction;

        public DashControlArgs(MovementControlArgs args, MovementDirection direction) : base(args)
        {
            _direction = direction;
        }
    }
}