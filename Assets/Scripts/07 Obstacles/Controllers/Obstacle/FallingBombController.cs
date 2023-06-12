using UnityEngine;

using JumpMaster.LevelTrackers;

using Studio28.Utility;
using Studio28.Probability;

namespace JumpMaster.Obstacles
{
    public class FallingBombController : ObstacleController<FallingBomb, FallingBombSO, FallingBombSpawnSO, FallingBombSpawnMetricsSO>
    {
        private enum State { FIRST_INTERVAL, SECOND_INTERVAL, AVAILABLE }
        private readonly StateMachine<State> _stateController;

        private Randomized _spawnProbability;

        protected override SpawnArgs GenerateSpawnArguments(FallingBombSpawnSO spawn_data)
        {
            Vector2 position;
            bool success = ObstacleLevelController.SpawnPoints.TryGetRandomPoint(
                ObstacleSpawnPointTracker.EdgePosition.HORIZONTAL_TOP,
                _spawnMetrics.Interval, out position);
            if (!success)
            {
                TimeTracker.Instance.StartTimeTracking(() => { _stateController.SetState(State.AVAILABLE); }, _spawnMetrics.SecondaryInterval);
                _stateController.SetState(State.SECOND_INTERVAL);
                return null;
            }
            return new(position);
        }

        protected override bool CanSpawn()
        {
            if (!_stateController.CurrentState.Equals(State.AVAILABLE))
                return false;
            if (_spawnProbability.Outcome())
                return true;
            TimeTracker.Instance.StartTimeTracking(() => { _stateController.SetState(State.AVAILABLE); }, _spawnMetrics.SecondaryInterval);
            _stateController.SetState(State.SECOND_INTERVAL);
            return false;
        }

        public FallingBombController(FallingBombSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics)
        {
            _spawnProbability = new(default_spawn_metrics.SpawnChance);
            _stateController = new("Falling Bomb Controller", State.AVAILABLE);
        }

        protected override void OnUpdateData()
        {
            _spawnProbability = new(_spawnMetrics.SpawnChance);
        }

        protected override void PostSpawn(FallingBomb obstacle)
        {
            TimeTracker.Instance.StartTimeTracking(() => { _stateController.SetState(State.AVAILABLE); }, _spawnMetrics.Interval);
            _stateController.SetState(State.FIRST_INTERVAL);
        }
    }
}