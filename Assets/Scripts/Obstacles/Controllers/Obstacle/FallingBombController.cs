using UnityEngine;

using Studio28.Probability;

namespace JumpMaster.Obstacles
{
    public class FallingBombController : ObstacleController<FallingBomb, FallingBombSO, FallingBombSpawnSO, FallingBombSpawnMetricsSO, FallingBombArgs>
    {
        private Randomized _spawnProbability;

        protected override FallingBombArgs GenerateSpawnArguments()
        {
            int spawn_point = GetSpawnPoint();
            return new FallingBombArgs(SpawnPositionAtPoint(spawn_point), spawn_point);
        }

        protected override bool CanSpawn()
        {
            if (LastSpawnTime > 0f && Time.time - LastSpawnTime < _spawnMetrics.Interval)
                return false;

            if (_spawnProbability.Outcome())
                return true;
            else if (Time.time - LastSpawnTime >= _spawnMetrics.Interval)
                return false;

            return false;
        }

        public FallingBombController(FallingBombSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics)
        {
            _spawnProbability = new(default_spawn_metrics.SpawnChance);
        }

        protected override void OnUpdateData()
        {
            _spawnProbability = new(_spawnMetrics.SpawnChance);
        }

        private bool SpawnPointActive(int point)
        {
            if (ActiveObstacles.Length == 0)
                return false;

            FallingBomb bomb;
            for (int i = 0; i < AllObstacles.Length; i++)
            {
                bomb = (FallingBomb)AllObstacles[i];
                if (!bomb.Spawned)
                    continue;
                if (bomb.SpawnArgs.SpawnPositionOrder == point)
                    return true;
            }
            return false;
        }

        private int GetSpawnPoint()
        {
            int point = Random.Range(0, _spawnMetrics.SpawnPoints - 1);
            if (SpawnPointActive(point))
                return GetSpawnPoint();
            return point;
        }

        private Vector2 SpawnPositionAtPoint(int spawn_point)
        {
            Vector2 position_screen;

            float pos = (float)Screen.width / (_spawnMetrics.SpawnPoints + 1);

            position_screen = new(pos * (spawn_point + 1), Screen.height);

            return position_screen;
        }
    }
}