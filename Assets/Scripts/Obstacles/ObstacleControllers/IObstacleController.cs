using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    public interface IObstacleController<Obstacle, ObstacleScriptableObject, SpawnScriptableObject, SpawnMetricsScriptableObject, SpawnArguments>
        where Obstacle : JumpMaster.Obstacles.Obstacle
        where ObstacleScriptableObject : ObstacleSO
        where SpawnScriptableObject : SpawnSO
        where SpawnMetricsScriptableObject : SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject>
        where SpawnArguments : SpawnArgs
    {
        public ObstacleControllerData<Obstacle, ObstacleScriptableObject, SpawnScriptableObject, SpawnMetricsScriptableObject, SpawnArguments> ControllerData { get; }
    }
}