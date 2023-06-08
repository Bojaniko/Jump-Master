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

        /// <summary>
        /// Called when an obstacle is spawned.
        /// </summary>
        public event ObstacleControllerEventHandler OnActiveObstaclesChange;

        public ISpawnMetricsSO SpawnMetrics => _spawnMetrics;
        protected SpawnMetricsScriptableObject _spawnMetrics;
        public readonly SpawnMetricsScriptableObject DefaultSpawnMetrics;

        /// <summary>
        /// The last time when an obstacle was spawned.
        /// </summary>
        public float LastSpawnTime { get; private set; }

        /// <summary>
        /// The amount of obstacles currently spawned.
        /// </summary>
        public int ObstaclesSpawned { get; private set; }

        /// <summary>
        /// The amount of obstacles available to spawn.
        /// </summary>
        public int SpawnsLeft
        {
            get
            {
                if (SpawnMetrics == null) return 0;
                return SpawnMetrics.SpawnAmount - ObstaclesSpawned;
            }
        }

        /// <summary>
        /// Try to spawn an obstacle held by the controller based on the
        /// criterium of the CanSpawn method, and on other data defined by the obstacle.
        /// </summary>
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
                SpawnArguments args = GenerateSpawnArguments();
                if (args == null)
                    return;
                ObstacleType spawn = SpawnFromPool(_spawnMetrics.GetRandomSpawnData(), args, _spawnMetrics);
                PostSpawn(spawn);
            }
        }
        /// <summary>
        /// Generate the spawn arguments for the current obstacle being spawned.
        /// Will cancel spawning if returns null.
        /// </summary>
        protected abstract SpawnArguments GenerateSpawnArguments();
        /// <summary>
        /// The obstacle controller specific requirements for spawning an obstacle.
        /// </summary>
        protected abstract bool CanSpawn();
        /// <summary>
        /// Called after an obstacle has been spawned.
        /// </summary>
        /// <param name="obstacle">The spawned obstacle.</param>
        protected abstract void PostSpawn(ObstacleType obstacle);

        // ##### POOLING AND DATA ##### \\

        public int ObstaclesInPool => Pool.ObstaclesInPool;
        public ObstacleScriptableObject ObstacleData => Pool.Data;
        public IObstacle[] ActiveObstacles => Pool.ActiveObstacles as IObstacle[];
        public IObstacle[] AllObstacles => Pool.AllObstacles as IObstacle[];

        private readonly ObstaclePool<ObstacleType, ObstacleScriptableObject> Pool;

        /// <summary>
        /// Changes the used spawn metrics data.
        /// </summary>
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
            OnUpdateData();
        }

        /// <summary>
        /// Called when the spawn metrics data is updated by the obstacle level controller.
        /// </summary>
        protected abstract void OnUpdateData();

        private ObstacleType SpawnFromPool(SpawnScriptableObject spawn_data, SpawnArguments spawn_args, SpawnMetricsScriptableObject spawn_metrics)
        {
            ObstaclesSpawned++;
            LastSpawnTime = Time.time;
            ObstacleType spawn = Pool.PullObstacle();
            spawn.Spawn(spawn_data, spawn_args, spawn_metrics);
            return spawn;
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