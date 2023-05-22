using UnityEngine;

namespace JumpMaster.Obstacles
{
    public delegate void ObstacleStateEventHandler(IObstacle obstacle);

    public interface IObstacle
    {
        public bool Spawned { get; }
        public void Spawn(SpawnSO spawn_data, SpawnArgs spawn_args, ISpawnMetricsSO spawn_metrics);
        public void Despawn();

        public event ObstacleStateEventHandler OnSpawn;
        public event ObstacleStateEventHandler OnDespawn;
        public event ObstacleStateEventHandler OnMarginPositionChange;

        public bool InTopMargin { get; }
        public bool InBottomMargin { get; }
        public bool InLeftMargin { get; }
        public bool InRightMargin { get; }

        public bool BoundsUnderScreen { get; }
        public bool BoundsOverScreen { get; }
        public bool BoundsLeftOfScreen { get; }
        public bool BoundsRightOfScreen { get; }
        public Vector2 ScreenPosition { get; }

        public void Generate(ObstacleSO data, IObstacleController controller);
    }
}