namespace JumpMaster.Obstacles
{
    public interface ISpawnMetricsSO
    {
        /// <summary>
        /// The random generation probability weight.
        /// </summary>
        public int SpawnWeight { get; }

        /// <summary>
        /// Amount to spawn in a wave.
        /// </summary>
        public int SpawnAmount { get; }

        /// <summary>
        /// Max active of this type at any given time.
        /// </summary>
        public int MaxActiveObstacles { get; }

        /// <summary>
        /// The cooldown for spawn points.
        /// </summary>
        public float SpawnPointCooldown { get; }
    }
}