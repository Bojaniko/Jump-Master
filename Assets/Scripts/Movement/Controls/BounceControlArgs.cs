using UnityEngine;

namespace JumpMaster.Movement
{
    public class BounceControlArgs : MovementControlArgs
    {
        public readonly Vector2 Direction;

        public BounceControlArgs(MovementControlArgs args) : base(args)
        {
            Direction = Vector2.up;
        }

        public BounceControlArgs(MovementControlArgs args, Vector2 direction) : base(args)
        {
            Direction = direction;
        }
    }
}