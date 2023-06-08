using UnityEngine;

namespace JumpMaster.Obstacles
{
    public abstract class SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject> : SpawnMetricsBaseSO
        where ObstacleScriptableObject : ObstacleSO
        where SpawnScriptableObject : SpawnSO
    {
        [SerializeField, Range(0, 10), Tooltip("The random generation probability weight.")] private int _spawnWeight = 5;
        [SerializeField, Range(1,100), Tooltip("Amount to spawn in a wave")] private int _spawnAmount = 10;
        [SerializeField, Range(1, 20), Tooltip("Max active of this type at any given time.")] private int _maxActiveObstacles = 3;
        [SerializeField, Range(0f, 50f), Tooltip("The cooldown for spawn points.")] private float _spawnPointCooldown = 1f;

        protected override int b_spawnWeight => _spawnWeight;
        protected override int b_spawnAmount => _spawnAmount;
        protected override int b_maxActiveObstacles => _maxActiveObstacles;
        protected override float b_spawnPointCooldown => _spawnPointCooldown;

        public ObstacleScriptableObject Data;
        public SpawnScriptableObject[] SpawnData;

        public SpawnScriptableObject GetRandomSpawnData()
        {
            int random = Random.Range(0, SpawnData.Length);
            return SpawnData[random];
        }
    }
}