namespace JumpMaster.Obstacles
{
    public delegate void ObstacleControllerEventHandler();

    public interface IObstacleController
    {
        public event ObstacleControllerEventHandler OnActiveObstaclesChange;

        public int SpawnsLeft { get; }

        public IObstacle[] AllObstacles { get; }
        public IObstacle[] ActiveObstacles { get; }

        public ISpawnMetricsSO SpawnMetrics { get; }

        public void TrySpawn();
        public void UpdateData(ISpawnMetricsSO spawn_metrics);
    }
}