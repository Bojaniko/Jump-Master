using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Falling Bomb Spawn Data", menuName = "Game/Obstacles/Spawn/Falling Bomb")]
    public class FallingBombSpawnSO : SpawnSO
    {
        [Range(0.1f, 10f)]
        public float FallSpeed = 1.5f;
        [Range(0.1f, 10f)]
        public float DetectionRadius = 5f;
        [Range(1.5f, 10f)]
        public float DetectionShowDistance = 7f;

        [Header("Explosion")]
        [Range(100, 2000)]
        public int ArmingDurationMS = 300;
    }
}