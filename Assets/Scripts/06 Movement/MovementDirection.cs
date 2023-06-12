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

        /// <summary>
        /// Direction of (0, 0)
        /// </summary>
        public static MovementDirection Zero { get { return new(0, 0); } }
        /// <summary>
        /// Direction of (1, 0)
        /// </summary>
        public static MovementDirection Up { get { return new(1, 0); } }
        /// <summary>
        /// Direction of (-1, 0)
        /// </summary>
        public static MovementDirection Down { get { return new(-1, 0); } }
        /// <summary>
        /// Direction of (0, -1)
        /// </summary>
        public static MovementDirection Left { get { return new(0, -1); } }
        /// <summary>
        /// Direction of (0, 1);
        /// </summary>
        public static MovementDirection Right { get { return new(0, 1); } }
    }
}