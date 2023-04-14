using System.Collections.Generic;

using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class MissileController : ObstacleController<Missile, MissileSO, MissileSpawnSO, MissileSpawnMetricsSO, MissileSpawnArgs>
    {
        private int _spawnRandomTarget;
        private List<int> _randomNumbers;

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

        protected override MissileSpawnArgs GenerateSpawnArguments()
        {
            MissileDirection direction = RandomDirection;
            return new MissileSpawnArgs(RandomSpawnPosition(direction), direction);
        }

        public MissileController(MissileSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics)
        {
            _randomNumbers = new();
            _spawnRandomTarget = Random.Range(0, 100);
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

        private bool ScreenPositionOverlaping(Vector3 position, MissileDirection direction)
        {
            if (ActiveObstacles.Length == 0)
                return false;
            foreach (Missile missile in ActiveObstacles)
            {
                switch(direction)
                {
                    case MissileDirection.DOWN:
                        if (position.x > missile.SpawnArgs.ScreenPosition.x + _spawnMetrics.ScreenOverlapLimit ||
                            position.x < missile.SpawnArgs.ScreenPosition.x - _spawnMetrics.ScreenOverlapLimit)
                            return false;
                        break;

                    case MissileDirection.UP:
                        if (position.x > missile.SpawnArgs.ScreenPosition.x + _spawnMetrics.ScreenOverlapLimit ||
                            position.x < missile.SpawnArgs.ScreenPosition.x - _spawnMetrics.ScreenOverlapLimit)
                            return false;
                        break;

                    case MissileDirection.LEFT:
                        if (position.y > missile.SpawnArgs.ScreenPosition.y + _spawnMetrics.ScreenOverlapLimit ||
                            position.y < missile.SpawnArgs.ScreenPosition.y - _spawnMetrics.ScreenOverlapLimit)
                            return false;
                        break;

                    case MissileDirection.RIGHT:
                        if (position.y > missile.SpawnArgs.ScreenPosition.y + _spawnMetrics.ScreenOverlapLimit ||
                            position.y < missile.SpawnArgs.ScreenPosition.y - _spawnMetrics.ScreenOverlapLimit)
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
                    percent_to_pos = (Screen.width / 100) * _spawnMetrics.ScreenMarginPercentage;
                    position_screen = new(Random.Range(percent_to_pos, Screen.width - percent_to_pos), Screen.height, ObstacleData.Z_Position);

                    if (ScreenPositionOverlaping(position_screen, direction))
                        return RandomSpawnPosition(direction);
                    break;
                case MissileDirection.UP:
                    percent_to_pos = (Screen.width / 100) * _spawnMetrics.ScreenMarginPercentage;
                    position_screen = new(Random.Range(percent_to_pos, Screen.width - percent_to_pos), 0, ObstacleData.Z_Position);

                    if (ScreenPositionOverlaping(position_screen, direction))
                        return RandomSpawnPosition(direction);
                    break;
                case MissileDirection.LEFT:
                    percent_to_pos = (Screen.height / 100) * _spawnMetrics.ScreenMarginPercentage;
                    position_screen = new(Screen.width, Random.Range(percent_to_pos, Screen.height - percent_to_pos), ObstacleData.Z_Position);

                    if (ScreenPositionOverlaping(position_screen, direction))
                        return RandomSpawnPosition(direction);
                    break;
                case MissileDirection.RIGHT:
                    percent_to_pos = (Screen.height / 100) * _spawnMetrics.ScreenMarginPercentage;
                    position_screen = new(0, Random.Range(percent_to_pos, Screen.height - percent_to_pos), ObstacleData.Z_Position);

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