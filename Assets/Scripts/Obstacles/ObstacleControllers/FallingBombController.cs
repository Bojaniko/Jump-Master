using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class FallingBombController : ObstacleController<FallingBomb, FallingBombSO, FallingBombSpawnSO, FallingBombSpawnMetricsSO, FallingBombArgs>
    {
        private int _spawnRandomTarget;
        private List<int> _randomNumbers;

        protected override FallingBombArgs GenerateSpawnArguments()
        {
            int spawn_point = GetSpawnPoint();
            return new FallingBombArgs(SpawnPositionAtPoint(spawn_point), spawn_point);
        }

        protected override bool CanSpawn()
        {
            if (LastSpawnTime > 0f && Time.time - LastSpawnTime < _spawnMetrics.Interval)
                return false;

            if (SpawnChanceSuccessful())
                return true;
            else if (Time.time - LastSpawnTime >= _spawnMetrics.Interval)
                return false;

            return false;
        }

        public FallingBombController(FallingBombSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics)
        {
            _spawnRandomTarget = Random.Range(0, 100);
            _randomNumbers = new();
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

        private int GetUniqueRandomPercentage()
        {
            int num = Random.Range(0, 100);

            if (_randomNumbers.Contains(num))
                return GetUniqueRandomPercentage();
            else
                return num;
        }

        private bool SpawnChanceSuccessful()
        {
            if (_spawnMetrics.SpawnChance == 100)
                return true;

            _randomNumbers.Clear();

            for (int i = 0; i < _spawnMetrics.SpawnChance + 1; i++)
            {
                _randomNumbers.Add(GetUniqueRandomPercentage());
                if (_randomNumbers.Contains(_spawnRandomTarget))
                    return true;
            }
            return false;
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