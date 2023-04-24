using UnityEngine;

namespace JumpMaster.Movement
{
    public readonly struct MovementDirection
    {
        public readonly int Vertical;
        public readonly int Horizontal;
        public MovementDirection(int vertical, int horizontal)
        {
            Vertical = Mathf.Clamp(vertical, -1, 1);
            Horizontal = Mathf.Clamp(horizontal, -1, 1);
        }

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(this.GetType()))
                return false;
            MovementDirection direction_compare = (MovementDirection)obj;
            return (Vertical == direction_compare.Vertical && Horizontal == direction_compare.Horizontal);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static MovementDirection Zero { get { return new(0, 0); } }
        public static MovementDirection Up { get { return new(1, 0); } }
        public static MovementDirection Down { get { return new(-1, 0); } }
        public static MovementDirection Left { get { return new(0, -1); } }
        public static MovementDirection Right { get { return new(0, 1); } }
    }
}