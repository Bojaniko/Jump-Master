using UnityEngine;

namespace JumpMaster.Obstacles
{
    public sealed class SpawnArgs
    {
        public Vector2 ScreenPosition => _screenPosition;
        private readonly Vector2 _screenPosition;

        public SpawnArgs(Vector2 screen_position)
        {
            _screenPosition = screen_position;
        }
    }
}