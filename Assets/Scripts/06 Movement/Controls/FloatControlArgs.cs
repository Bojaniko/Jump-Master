using UnityEngine;

namespace JumpMaster.Movement
{
    public class FloatControlArgs : MovementControlArgs
    {
        public readonly MovementDirection Direction;

        public FloatControlArgs(MovementControlArgs args, MovementDirection direction) : base(args)
        {
            Direction = direction;
        }
    }
}