using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    public class FallingBombController : ObstacleController, IObstacleController<FallingBomb, FallingBombSO, FallingBombSpawnSO, FallingBombSpawnMetricsSO, FallingBombArgs>
    {
        private ObstacleControllerData<FallingBomb, FallingBombSO, FallingBombSpawnSO, FallingBombSpawnMetricsSO, FallingBombArgs> _data;
        public ObstacleControllerData<FallingBomb, FallingBombSO, FallingBombSpawnSO, FallingBombSpawnMetricsSO, FallingBombArgs> ControllerData { get { return _data; } }

        private int _spawnRandomTarget;
        private List<int> _randomNumbers;

        protected override void Spawn()
        {
            int spawn_point = GetSpawnPoint();
            FallingBombArgs spawn_args = new FallingBombArgs(SpawnPositionAtPoint(spawn_point), spawn_point);

            _data.SpawnFromPool(_data.SpawnMetrics.GetRandomSpawnData(), spawn_args);
        }

        protected override bool CanSpawn()
        {
            if (_data.LastSpawnTime > 0f && Time.time - _data.LastSpawnTime < _data.SpawnMetrics.Interval)
                return false;

            if (SpawnChanceSuccessful())
                return true;
            else if (Time.time - _data.LastSpawnTime >= _data.SpawnMetrics.Interval)
                return false;

            return false;
        }

        public FallingBombController(FallingBombSpawnMetricsSO default_spawn_metrics)
        {
            _spawnRandomTarget = Random.Range(0, 100);
            _randomNumbers = new();

            _data = new(default_spawn_metrics, this);
        }

        private bool SpawnPointActive(int point)
        {
            if (_data.ActiveObstacles.Length == 0)
                return false;

            FallingBomb bomb;
            for (int i = 0; i < _data.AllObstacles.Length; i++)
            {
                bomb = _data.AllObstacles[i];
                if (!bomb.SpawnController.Spawned)
                    continue;
                if (bomb.SpawnController.SpawnArgs.SpawnPositionOrder == point)
                    return true;
            }
            return false;
        }

        private int GetSpawnPoint()
        {
            int point = Random.Range(0, _data.SpawnMetrics.SpawnPoints - 1);
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
            if (_data.SpawnMetrics.SpawnChance == 100)
                return true;

            _randomNumbers.Clear();

            for (int i = 0; i < _data.SpawnMetrics.SpawnChance + 1; i++)
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

            float pos = Screen.width / (_data.SpawnMetrics.SpawnPoints + 1);

            position_screen = new(pos * (spawn_point + 1), Screen.height);

            return position_screen;
        }
    }
}