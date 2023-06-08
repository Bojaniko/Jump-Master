using UnityEngine;

namespace JumpMaster.Controls
{
    internal struct SwipeInput
    {
        public float EndTime { get; set; }
        public float StartTime { get; set; }

        public Vector2 EndPosition { get; set; }
        public Vector2 StartPosition { get; set; }

        public bool Delay { get; set; }
    }
}
