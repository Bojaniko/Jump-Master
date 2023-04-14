using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class LaserGateController : ObstacleController<LaserGate, LaserGateSO, LaserGateSpawnSO, LaserGateSpawnMetricsSO, LaserGateSpawnArgs>
    {
        private float _lastSpawnTime = 0f;

        protected override LaserGateSpawnArgs GenerateSpawnArguments()
        {
            Vector3 screen_position = new Vector3(Screen.width * 0.5f, Screen.height, ObstacleData.Z_Position);
            return new(screen_position, Camera.main.ScreenToWorldPoint(screen_position));
        }

        protected override bool CanSpawn()
        {
            if (Time.time - _lastSpawnTime >= _spawnMetrics.Interval)
            {
                _lastSpawnTime = Time.time;
                if (ObstacleLevelController.Instance.ActiveObstaclesInTopSpawnMargin.Length == 0)
                    return true;
            }

            return false;
        }

        public LaserGateController(LaserGateSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics)
        {

        }
    }
}