using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject> : ScriptableObject, ISpawnMetricsSO
        where ObstacleScriptableObject : ObstacleSO
        where SpawnScriptableObject : SpawnSO
    {
        [SerializeField, Range(1,100)] private int _spawnAmount = 10;
        [SerializeField, Range(1, 20)] private int _maxActiveObstacles = 3;

        public int SpawnAmount => _spawnAmount;
        public int MaxActiveObstacles => _maxActiveObstacles;

        public ObstacleScriptableObject Data;
        public SpawnScriptableObject[] SpawnData;

        public SpawnScriptableObject GetRandomSpawnData()
        {
            int random = Random.Range(0, SpawnData.Length - 1);

            return SpawnData[random];
        }
    }
}