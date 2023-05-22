namespace JumpMaster.Obstacles
{
    public interface ISpawnMetricsSO
    {
        public int SpawnWeight { get; }
        public int SpawnAmount { get; }
        public int MaxActiveObstacles { get; }
    }
}