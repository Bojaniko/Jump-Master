using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    public enum ObstacleControllerType { FallingBomb, LaserGate, Missile }

    [CreateAssetMenu(fileName = "Controllers Data", menuName = "Game/Obstacles/Controllers/Controllers Data")]
    public class ObstacleControllersSO : ScriptableObject
    {
        public ObstacleControllerType SelectedObstacle;

        public ObstacleControllerType[] SelectedObstacles;

        public FallingBombSpawnMetricsSO FallingBomb;

        public LaserGateSpawnMetricsSO LaserGate;

        public MissileSpawnMetricsSO Missile;
    }
}