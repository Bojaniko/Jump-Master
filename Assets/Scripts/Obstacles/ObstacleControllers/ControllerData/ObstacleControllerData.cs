using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    public class ObstacleControllerData<ObstacleType, ObstacleScriptableObject, SpawnScriptableObject, SpawnMetricsScriptableObject, SpawnArguments>
            where ObstacleType : Obstacle
            where ObstacleScriptableObject : ObstacleSO
            where SpawnScriptableObject : SpawnSO
            where SpawnMetricsScriptableObject : SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject>
            where SpawnArguments : SpawnArgs
    {
        public int ObstaclesSpawned { get; private set; }
        public float LastSpawnTime { get; private set; }
        public SpawnMetricsScriptableObject SpawnMetrics { get; private set; }
        public readonly SpawnMetricsScriptableObject DefaultSpawnMetrics;
        private readonly ObstaclePool<ObstacleType, ObstacleScriptableObject, SpawnScriptableObject, SpawnArguments> Pool;

        public ObstacleControllerData(SpawnMetricsScriptableObject default_spawn_metrics, ObstacleController controller)
        {
            ObstaclesSpawned = 0;
            LastSpawnTime = Time.time;
            SpawnMetrics = null;
            DefaultSpawnMetrics = default_spawn_metrics;
            Pool = new(DefaultSpawnMetrics.Data, controller, DefaultSpawnMetrics.MaxActiveObstacles);
        }

        public void UpdateData(SpawnMetricsScriptableObject spawn_metrics)
        {
            ObstaclesSpawned = 0;
            SpawnMetrics = spawn_metrics;
            if (SpawnMetrics != null && Pool.AllObstacles.Length > SpawnMetrics.MaxActiveObstacles)
                Pool.GenerateNewObstacles(Pool.AllObstacles.Length - SpawnMetrics.MaxActiveObstacles);
        }

        public void SpawnFromPool(SpawnScriptableObject spawn_data, SpawnArguments spawn_args)
        {
            ObstaclesSpawned++;
            LastSpawnTime = Time.time;
            Pool.SpawnObstacle(spawn_data, spawn_args);
        }

        public int ObstaclesInPool
        {
            get
            {
                return Pool.ObstaclesInPool;
            }
        }
        public ObstacleScriptableObject ObstacleData
        {
            get
            {
                return Pool.Data;
            }
        }
        public ObstacleType[] ActiveObstacles
        {
            get
            {
                return Pool.ActiveObstacles;
            }
        }
        public ObstacleType[] AllObstacles
        {
            get
            {
                return Pool.AllObstacles;
            }
        }
    }
}