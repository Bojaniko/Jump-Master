using UnityEngine;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Obstacles
{
    public class ElectroBallController : ObstacleController<ElectroBall, ElectroBallSO, ElectroBallSpawnSO, ElectroBallSpawnMetricsSO>
    {
        public ElectroBallController(ElectroBallSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics)
        {
            _waitingForInterval = false;
        }

        private bool _waitingForInterval;
        protected override bool CanSpawn()
        {
            if (ActiveObstacles.Length == 1 || _waitingForInterval)
                return false;
            return true;
        }
        protected override void PostSpawn(ElectroBall obstacle)
        {
            obstacle.OnDespawn += StartIntervalCountdown; 
        }

        private void StartIntervalCountdown(IObstacle obstacle)
        {
            _waitingForInterval = true;
            TimeTracker.Instance.StartTimeTracking(EndIntervalCountdown, _spawnMetrics.GetInterval());
        }
        private void EndIntervalCountdown() => _waitingForInterval = false;

        protected override SpawnArgs GenerateSpawnArguments(ElectroBallSpawnSO spawn_data)
        {
            Vector2 position;
            ObstacleSpawnPointTracker.EdgePosition edgePosition;
            bool success = ObstacleLevelController.SpawnPoints.TryGetAnyRandomPoint(SpawnMetrics.SpawnPointCooldown, out position, out edgePosition);
            if (!success)
                return null;
            return new(position);
        }

        protected override void OnUpdateData()
        {

        }
    }
}