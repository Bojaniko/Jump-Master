using System.Collections;

using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    public class LaserGateController : ObstacleController, IObstacleController<LaserGate, LaserGateSO, LaserGateSpawnSO, LaserGateSpawnMetricsSO, LaserGateSpawnArgs>
    {
        private ObstacleControllerData<LaserGate, LaserGateSO, LaserGateSpawnSO, LaserGateSpawnMetricsSO, LaserGateSpawnArgs> _data;
        public ObstacleControllerData<LaserGate, LaserGateSO, LaserGateSpawnSO, LaserGateSpawnMetricsSO, LaserGateSpawnArgs> ControllerData { get { return _data; } }

        private LaserGate _firstSpawn;

        private float _lastSpawnTime = 0f;

        protected override void Spawn()
        {
            Vector3 screen_position = new Vector3(Screen.width * 0.5f, Screen.height, _data.ObstacleData.Z_Position);
            LaserGateSpawnArgs spawn_args = new(screen_position, Camera.main.ScreenToWorldPoint(screen_position));

            _data.SpawnFromPool(_data.SpawnMetrics.GetRandomSpawnData(), spawn_args);
        }

        protected override bool CanSpawn()
        {
            if (Time.time - _lastSpawnTime >= _data.SpawnMetrics.Interval)
            {
                _lastSpawnTime = Time.time;
                if (ObstacleLevelController.Instance.ActiveObstaclesInTopSpawnMargin.Length == 0)
                    return true;
            }

            return false;
        }

        public LaserGateController(LaserGateSpawnMetricsSO default_spawn_metrics)
        {
            _data = new(default_spawn_metrics, this);
        }
    }
}