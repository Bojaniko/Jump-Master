using UnityEngine;

namespace JumpMaster.Obstacles
{
    public abstract class SpawnMetricsBaseSO : ScriptableObject, ISpawnMetricsSO
    {
        protected abstract int b_spawnWeight { get; }
        protected abstract int b_spawnAmount { get; }
        protected abstract int b_maxActiveObstacles { get; }
        protected abstract float b_spawnPointCooldown { get; }

        public int SpawnWeight => b_spawnWeight;
        public int SpawnAmount => b_spawnAmount;
        public int MaxActiveObstacles => b_maxActiveObstacles;
        public float SpawnPointCooldown => b_spawnPointCooldown;
    }
}