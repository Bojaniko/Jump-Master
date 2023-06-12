using UnityEngine;

namespace JumpMaster.Movement
{
    public class HangControlArgs : MovementControlArgs
    {
        public MovementDirection Direction;

        public HangControlArgs(MovementControlArgs args, MovementDirection direction) : base(args)
        {
            Direction = direction;
        }
    }
}