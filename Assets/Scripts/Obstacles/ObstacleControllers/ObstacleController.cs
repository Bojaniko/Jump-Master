using UnityEngine;

namespace JumpMaster.Obstacles
{
    public abstract class ObstacleController<ObstacleType, ObstacleScriptableObject, SpawnScriptableObject, SpawnMetricsScriptableObject, SpawnArguments> : IObstacleController
        where ObstacleType : IObstacle
        where ObstacleScriptableObject : ObstacleSO
        where SpawnScriptableObject : SpawnSO
        where SpawnMetricsScriptableObject : SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject>
        where SpawnArguments : SpawnArgs
    {
        protected ObstacleController(SpawnMetricsScriptableObject default_spawn_metrics)
        {
            ObstaclesSpawned = 0;
            LastSpawnTime = Time.time;
            _spawnMetrics = null;
            DefaultSpawnMetrics = default_spawn_metrics;

            Pool = new(DefaultSpawnMetrics.Data, this, DefaultSpawnMetrics.MaxActiveObstacles);
            SubscribeObstacleEvents(Pool.AllObstacles);
            Pool.OnActiveObstaclesChange += () =>
            {
                OnActiveObstaclesChange?.Invoke();
            };
        }

        // ##### SPAWNING ##### \\

        public event ObstacleControllerEventHandler OnActiveObstaclesChange;

        public ISpawnMetricsSO SpawnMetrics => _spawnMetrics;
        protected SpawnMetricsScriptableObject _spawnMetrics;
        public readonly SpawnMetricsScriptableObject DefaultSpawnMetrics;

        public float LastSpawnTime { get; private set; }
        public int ObstaclesSpawned { get; private set; }
        public int SpawnsLeft
        {
            get
            {
                if (SpawnMetrics == null) return 0;
                return SpawnMetrics.SpawnAmount - ObstaclesSpawned;
            }
        }

        public void TrySpawn()
        {
            if (SpawnMetrics == null)
                return;

            if (ObstaclesInPool == 0)
                return;

            if (ActiveObstacles.Length == SpawnMetrics.MaxActiveObstacles)
                return;

            if (ObstaclesSpawned >= SpawnMetrics.SpawnAmount)
                return;

            if (CanSpawn())
            {
                SpawnFromPool(_spawnMetrics.GetRandomSpawnData(), GenerateSpawnArguments());
            }
        }
        protected abstract SpawnArguments GenerateSpawnArguments();
        protected abstract bool CanSpawn();

        // ##### POOLING AND DATA ##### \\

        public int ObstaclesInPool => Pool.ObstaclesInPool;
        public ObstacleScriptableObject ObstacleData => Pool.Data;
        public IObstacle[] ActiveObstacles => Pool.ActiveObstacles as IObstacle[];
        public IObstacle[] AllObstacles => Pool.AllObstacles as IObstacle[];

        private readonly ObstaclePool<ObstacleType, ObstacleScriptableObject> Pool;

        public void UpdateData(ISpawnMetricsSO spawn_metrics)
        {
            if (spawn_metrics == null)
                return;
            if (!spawn_metrics.GetType().Equals(typeof(SpawnMetricsScriptableObject)))
                return;
            if (ActiveObstacles.Length > 0)
                return;
            UpdateData((SpawnMetricsScriptableObject)spawn_metrics);
        }

        private void UpdateData(SpawnMetricsScriptableObject spawn_metrics)
        {
            ObstaclesSpawned = 0;
            _spawnMetrics = spawn_metrics;

            if (SpawnMetrics != null && Pool.AllObstacles.Length > _spawnMetrics.MaxActiveObstacles)
            {
                ObstacleType[] generated_obstacles = Pool.GenerateNewObstacles(Pool.AllObstacles.Length - _spawnMetrics.MaxActiveObstacles);
                SubscribeObstacleEvents(generated_obstacles);
            }
        }

        private void SpawnFromPool(SpawnScriptableObject spawn_data, SpawnArguments spawn_args)
        {
            ObstaclesSpawned++;

            //Debug.Log($"Spawned {typeof(ObstacleType)}, {ObstaclesSpawned} times.");

            LastSpawnTime = Time.time;
            ObstacleType spawn = Pool.PullObstacle();
            spawn.Spawn(spawn_data, spawn_args);
        }

        private void SubscribeObstacleEvents(ObstacleType[] obstacles)
        {
            foreach (ObstacleType obstacle in obstacles)
            {
                obstacle.OnDespawn += Pool.PushObstacle;
            }
        }
    }
}