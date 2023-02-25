using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    public class MissileController : ObstacleController, IObstacleController<Missile, MissileSO, MissileSpawnSO, MissileSpawnMetricsSO, MissileSpawnArgs>
    {
        private ObstacleControllerData<Missile, MissileSO, MissileSpawnSO, MissileSpawnMetricsSO, MissileSpawnArgs> _data;
        public ObstacleControllerData<Missile, MissileSO, MissileSpawnSO, MissileSpawnMetricsSO, MissileSpawnArgs> ControllerData { get { return _data; } }

        private int _spawnRandomTarget;
        private List<int> _randomNumbers;

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

        protected override void Spawn()
        {
            MissileDirection direction = RandomDirection;
            MissileSpawnArgs spawn_args = new MissileSpawnArgs(RandomSpawnPosition(direction), direction);

            _data.SpawnFromPool(_data.SpawnMetrics.GetRandomSpawnData(), spawn_args);
        }

        public MissileController(MissileSpawnMetricsSO default_spawn_metrics)
        {
            _randomNumbers = new();
            _spawnRandomTarget = Random.Range(0, 100);

            _data = new(default_spawn_metrics, this);
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

        private bool ScreenPositionOverlaping(Vector3 position, MissileDirection direction)
        {
            if (_data.ActiveObstacles.Length == 0)
                return false;
            foreach (Missile missile in _data.AllObstacles)
            {
                if (!missile.SpawnController.Spawned)
                    continue;
                switch(direction)
                {
                    case MissileDirection.DOWN:
                        if (position.x > missile.SpawnController.SpawnArgs.ScreenPosition.x + _data.SpawnMetrics.ScreenOverlapLimit ||
                            position.x < missile.SpawnController.SpawnArgs.ScreenPosition.x - _data.SpawnMetrics.ScreenOverlapLimit)
                            return false;
                        break;

                    case MissileDirection.UP:
                        if (position.x > missile.SpawnController.SpawnArgs.ScreenPosition.x + _data.SpawnMetrics.ScreenOverlapLimit ||
                            position.x < missile.SpawnController.SpawnArgs.ScreenPosition.x - _data.SpawnMetrics.ScreenOverlapLimit)
                            return false;
                        break;

                    case MissileDirection.LEFT:
                        if (position.y > missile.SpawnController.SpawnArgs.ScreenPosition.y + _data.SpawnMetrics.ScreenOverlapLimit ||
                            position.y < missile.SpawnController.SpawnArgs.ScreenPosition.y - _data.SpawnMetrics.ScreenOverlapLimit)
                            return false;
                        break;

                    case MissileDirection.RIGHT:
                        if (position.y > missile.SpawnController.SpawnArgs.ScreenPosition.y + _data.SpawnMetrics.ScreenOverlapLimit ||
                            position.y < missile.SpawnController.SpawnArgs.ScreenPosition.y - _data.SpawnMetrics.ScreenOverlapLimit)
                            return false;
                        break;
                }
            }
            return true;
        }

        private Vector3 RandomSpawnPosition(MissileDirection direction)
        {
            Vector3 position_screen;
            float percent_to_pos;
            switch(direction)
            {
                case MissileDirection.DOWN:
                    percent_to_pos = (Screen.width / 100) * _data.SpawnMetrics.ScreenMarginPercentage;
                    position_screen = new(Random.Range(percent_to_pos, Screen.width - percent_to_pos), Screen.height, _data.ObstacleData.Z_Position);

                    if (ScreenPositionOverlaping(position_screen, direction))
                        return RandomSpawnPosition(direction);
                    break;
                case MissileDirection.UP:
                    percent_to_pos = (Screen.width / 100) * _data.SpawnMetrics.ScreenMarginPercentage;
                    position_screen = new(Random.Range(percent_to_pos, Screen.width - percent_to_pos), 0, _data.ObstacleData.Z_Position);

                    if (ScreenPositionOverlaping(position_screen, direction))
                        return RandomSpawnPosition(direction);
                    break;
                case MissileDirection.LEFT:
                    percent_to_pos = (Screen.height / 100) * _data.SpawnMetrics.ScreenMarginPercentage;
                    position_screen = new(Screen.width, Random.Range(percent_to_pos, Screen.height - percent_to_pos), _data.ObstacleData.Z_Position);

                    if (ScreenPositionOverlaping(position_screen, direction))
                        return RandomSpawnPosition(direction);
                    break;
                case MissileDirection.RIGHT:
                    percent_to_pos = (Screen.height / 100) * _data.SpawnMetrics.ScreenMarginPercentage;
                    position_screen = new(0, Random.Range(percent_to_pos, Screen.height - percent_to_pos), _data.ObstacleData.Z_Position);

                    if (ScreenPositionOverlaping(position_screen, direction))
                        return RandomSpawnPosition(direction);
                    break;
                default:
                    position_screen = new Vector3();
                    break;
            }
            return position_screen;
        }

        private MissileDirection RandomDirection
        {
            get
            {
                int direction = Random.Range(0, 4);
                switch(direction)
                {
                    case 0:
                        return MissileDirection.DOWN;
                    case 1:
                        return MissileDirection.UP;
                    case 2:
                        return MissileDirection.LEFT;
                    case 3:
                        return MissileDirection.RIGHT;
                }
                return MissileDirection.DOWN;
            }
        }
    }
}