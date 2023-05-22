using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Spawn Metrics", menuName = "Game/Obstacles/Metrics/Missile Spawn")]
    public class MissileSpawnMetricsSO : SpawnMetricsSO<MissileSO, MissileSpawnSO>
    {
        [Range(0, 100)]
        public int SpawnChance = 20;

        [Range(1f, 100f)]
        public float Interval = 5f;

        [Range(0, 50)]
        public int ScreenMarginPercentage = 20;

        [Range(0, 500)]
        public int ScreenOverlapLimit = 50;

        [Range(0f, 5f)]
        public float SpawnOffset = 0.5f;
    }
}