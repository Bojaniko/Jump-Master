using UnityEngine;

using Studio28.Probability;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Obstacles
{
    public class MissileController : ObstacleController<Missile, MissileSO, MissileSpawnSO, MissileSpawnMetricsSO, MissileSpawnArgs>
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

        protected override MissileSpawnArgs GenerateSpawnArguments()
        {
            Vector2 position;
            ObstacleSpawnPointTracker.EdgePosition edge;
            bool success = ObstacleLevelController.SpawnPoints.TryGetAnyRandomPoint(_spawnMetrics.Interval, out position, out edge);
            if (!success)
                return null;
            return new MissileSpawnArgs(position, GetDirection(edge));
        }

        private MissileDirection GetDirection(ObstacleSpawnPointTracker.EdgePosition edge)
        {
            if (edge.Equals(ObstacleSpawnPointTracker.EdgePosition.HORIZONTAL_TOP))
                return MissileDirection.DOWN;
            else if (edge.Equals(ObstacleSpawnPointTracker.EdgePosition.HORIZONTAL_BOTTOM))
                return MissileDirection.UP;
            else if (edge.Equals(ObstacleSpawnPointTracker.EdgePosition.VERTICAL_LEFT))
                return MissileDirection.RIGHT;
            else if (edge.Equals(ObstacleSpawnPointTracker.EdgePosition.VERTICAL_RIGHT))
                return MissileDirection.LEFT;
            return MissileDirection.DOWN;
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