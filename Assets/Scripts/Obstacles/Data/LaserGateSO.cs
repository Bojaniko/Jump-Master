using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Laser Gate Data", menuName = "Game/Obstacles/Data/Laser Gate")]
    public class LaserGateSO : ObstacleSO
    {
        [Range(0f, 2f)] public float LaserWidth = 0.05f;
        [Range(0.01f, 5f)] public float LaserAreaHeight = 1.1f;

        [ColorUsage(false, true)] public Color ActiveColor = Color.green;
        [ColorUsage(false, true)] public Color InactiveColor = Color.red;
    }
}