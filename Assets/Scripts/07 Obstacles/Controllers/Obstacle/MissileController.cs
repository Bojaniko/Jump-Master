using UnityEngine;

using Studio28.Probability;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Obstacles
{
    public class MissileController : ObstacleController<Missile, MissileSO, MissileSpawnSO, MissileSpawnMetricsSO>
    {
        protected override bool CanSpawn()
        {
            if (_intervalTimer != null)
                return false;
            bool outcome = _spawnProbability.Outcome();
            if (ObstaclesSpawned + 1 < SpawnMetrics.SpawnAmount || (ObstaclesSpawned + 1 == SpawnMetrics.SpawnAmount && !outcome))
                _intervalTimer = TimeTracker.Instance.StartTimeTracking(() => { _intervalTimer = null; }, _spawnMetrics.Interval);
            return outcome;
        }

        protected override SpawnArgs GenerateSpawnArguments(MissileSpawnSO spawn_data)
        {
            Vector2 position;
            bool success = ObstacleLevelController.SpawnPoints.TryGetRandomPoint(GetEdgeByDirection(spawn_data.Direction), _spawnMetrics.Interval, out position);
            if (!success)
                return null;
            return new SpawnArgs(position);
        }

        private ObstacleSpawnPointTracker.EdgePosition GetEdgeByDirection(MissileDirection direction)
        {
            if (direction.Equals(MissileDirection.DOWN))
                return ObstacleSpawnPointTracker.EdgePosition.HORIZONTAL_TOP;

            else if (direction.Equals(MissileDirection.UP))
                return ObstacleSpawnPointTracker.EdgePosition.HORIZONTAL_BOTTOM;

            else if (direction.Equals(MissileDirection.RIGHT))
                return ObstacleSpawnPointTracker.EdgePosition.VERTICAL_LEFT;

            else if (direction.Equals(MissileDirection.LEFT))
                return ObstacleSpawnPointTracker.EdgePosition.VERTICAL_RIGHT;
            return ObstacleSpawnPointTracker.EdgePosition.HORIZONTAL_TOP;
        }

        private TimeRecord _intervalTimer;
        private Randomized _spawnProbability;

        public MissileController(MissileSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics) =>
            _spawnProbability = new(default_spawn_metrics.SpawnChance);

        protected override void OnUpdateData() =>
            _spawnProbability = new(_spawnMetrics.SpawnChance);

        protected override void PostSpawn(Missile obstacle) { }
    }
}