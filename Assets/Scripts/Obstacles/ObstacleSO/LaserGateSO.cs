using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Laser Gate Data", menuName = "Game/Obstacles/Laser Gate")]
    public class LaserGateSO : ObstacleSO
    {
        [Range(0f, 2f)]
        public float LaserWidth = 0.05f;

        [ColorUsage(false, true)] public Color ActiveColor = Color.green;
        [ColorUsage(false, true)] public Color InactiveColor = Color.red;
    }
}