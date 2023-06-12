using UnityEngine;

namespace JumpMaster.Obstacles
{
    public abstract class SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject> : ScriptableObject, ISpawnMetricsSO
        where ObstacleScriptableObject : ObstacleSO
        where SpawnScriptableObject : SpawnSO
    {
        [SerializeField, Range(0, 10), Tooltip("The random generation probability weight.")] private int _spawnWeight = 5;
        [SerializeField, Range(1,100), Tooltip("Amount to spawn in a wave.")] private int _spawnAmount = 10;
        [SerializeField, Range(1, 20), Tooltip("Max active of this type at any given time.")] private int _maxActiveObstacles = 3;
        [SerializeField, Range(0f, 50f), Tooltip("The cooldown for spawn points.")] private float _spawnPointCooldown = 1f;

        public int SpawnWeight => _spawnWeight;
        public int SpawnAmount => _spawnAmount;
        public int MaxActiveObstacles => _maxActiveObstacles;
        public float SpawnPointCooldown => _spawnPointCooldown;

        public ObstacleScriptableObject Data => _data;
        [SerializeField] private ObstacleScriptableObject _data;

        public SpawnScriptableObject[] SpawnData => _spawnData;
        [SerializeField] private SpawnScriptableObject[] _spawnData;

        public SpawnScriptableObject GetRandomSpawnData()
        {
            int random = Random.Range(0, SpawnData.Length);
            return SpawnData[random];
        }

        public override string ToString()
        {
            return $"{name} ({GetType().Name})";
        }
    }
}