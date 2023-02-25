using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Falling Bomb Spawn Metrics", menuName = "Game/Obstacles/Metrics/Falling Bomb")]
    public class FallingBombSpawnMetricsSO : SpawnMetricsSO<FallingBombSO, FallingBombSpawnSO>
    {
        [Range(0, 100)]
        public int SpawnChance = 20;

        [Range(1f, 100f)]
        public float Interval = 5f;

        [Range(1, 20)]
        public int SpawnPoints = 4;

        [Range(0, 500)]
        public int ScreenOverlapLimit = 50;

        [Range(0f, 5f)]
        public float SpawnOffset = 0.5f;
    }
}