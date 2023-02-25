using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class SpawnArgs
    {
        public readonly Vector3 ScreenPosition;

        public SpawnArgs(Vector3 screen_position)
        {
            ScreenPosition = screen_position;
        }
    }

    public interface ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments>
        where ObstacleType : Obstacle
        where SpawnScriptableObject : SpawnSO
        where SpawnArguments : SpawnArgs
    {
        public ObstacleSpawnController<ObstacleType, SpawnScriptableObject, SpawnArguments, ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments>> SpawnController { get; }

        public ObstacleType ObstacleSelf { get; }
    }
}