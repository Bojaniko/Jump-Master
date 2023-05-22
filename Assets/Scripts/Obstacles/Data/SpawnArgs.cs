using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class SpawnArgs
    {
        public readonly Vector3 ScreenPosition;

        public SpawnArgs(Vector3 screen_position)
        {
            ScreenPosition = screen_position;
        }
    }
}