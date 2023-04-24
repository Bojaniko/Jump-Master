using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class FallControlArgs : MovementControlArgs
    {
        public readonly float Drag;

        public FallControlArgs(MovementControlArgs args) : base(args)
        {
            Drag = 0f;
        }

        public FallControlArgs(MovementControlArgs args, float drag) : base(args)
        {
            Drag = Mathf.Clamp(drag, 0f, Mathf.Infinity);
        }
    }
}