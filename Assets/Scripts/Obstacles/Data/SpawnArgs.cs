using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class SpawnArgs
    {
        public readonly Vector2 ScreenPosition;

        public SpawnArgs(Vector2 screen_position)
        {
            ScreenPosition = screen_position;
        }
    }
}