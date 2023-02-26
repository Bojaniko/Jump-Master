namespace JumpMaster.Obstacles
{
    public class ObstacleSpawnController<ObstacleType, SpawnScriptableObject, SpawnArguments, SpawnType>
        where ObstacleType : Obstacle
        where SpawnScriptableObject : SpawnSO
        where SpawnArguments : SpawnArgs
        where SpawnType : ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments>
    {
        public delegate void SpawnEventHandler();
        public event SpawnEventHandler OnSpawn;

        public delegate void DespawnEventHandler(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn);
        public event DespawnEventHandler OnDespawn;

        public bool Spawned { get; private set; }

        public SpawnScriptableObject SpawnData { get; private set; }

        public SpawnArguments SpawnArgs { get; private set; }

        private ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> _spawn;
        private ObstacleType _obstacle;

        public ObstacleSpawnController(SpawnType spawn, ObstacleType obstacle)
        {
            _spawn = spawn;
            _obstacle = obstacle;
            _obstacle.OnDespawnable += Despawn;
        }

        public void Spawn(SpawnScriptableObject spawn_data, SpawnArguments spawn_args)
        {
            if (Spawned)
                return;

            SpawnData = spawn_data;
            SpawnArgs = spawn_args;

            if (OnSpawn != null)
                OnSpawn();

            Spawned = true;
        }

        public void Despawn()
        {
            if (!Spawned)
                return;

            if (OnDespawn != null)
                OnDespawn(_spawn);

            SpawnData = null;
            SpawnArgs = null;
            Spawned = false;
        }
    }
}