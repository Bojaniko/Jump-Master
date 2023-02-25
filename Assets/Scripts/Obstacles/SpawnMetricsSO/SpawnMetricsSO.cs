using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject> : ScriptableObject
        where ObstacleScriptableObject : ObstacleSO
        where SpawnScriptableObject : SpawnSO
    {
        [Range(1, 100)]
        public int SpawnAmount = 10;

        [Range(1, 20)]
        public int MaxActiveObstacles = 3;

        public ObstacleScriptableObject Data;

        public SpawnScriptableObject[] SpawnData;

        public SpawnScriptableObject GetRandomSpawnData()
        {
            int random = Random.Range(0, SpawnData.Length - 1);

            return SpawnData[random];
        }
    }
}