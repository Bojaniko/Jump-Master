using UnityEngine;

namespace JumpMaster.Obstacles
{
    public delegate void ObstacleStateEventHandler(IObstacle obstacle);

    public interface IObstacle
    {
        public bool Spawned { get; }
        public void Spawn(SpawnSO spawn_data, SpawnArgs spawn_args);
        public void Despawn();

        public event ObstacleStateEventHandler OnSpawn;
        public event ObstacleStateEventHandler OnDespawn;

        public bool BoundsUnderScreen { get; }
        public bool BoundsOverScreen { get; }
        public bool BoundsLeftOfScreen { get; }
        public bool BoundsRightOfScreen { get; }
        public (Vector2 min, Vector2 max) BoundsScreenPosition { get; }

        public void Generate(ObstacleSO data, IObstacleController controller);
    }
}