using System.Collections.Generic;

using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class ObstaclePool<ObstacleType, ObstacleScriptableObject>
        where ObstacleType : IObstacle
        where ObstacleScriptableObject : ObstacleSO
    {
        public event ObstacleControllerEventHandler OnActiveObstaclesChange;

        public void PushObstacle(IObstacle spawn)
        {
            if (!spawn.GetType().Equals(typeof(ObstacleType)))
                return;

            ObstacleType type_spawn = (ObstacleType)spawn;

            if (!_activeObstacles.Contains(type_spawn))
                return;

            if (_obstacleQueue.Contains(type_spawn))
                return;

            _activeObstacles.Remove(type_spawn);
            _obstacleQueue.Enqueue(type_spawn);

            if (OnActiveObstaclesChange != null)
                OnActiveObstaclesChange();
        }

        public ObstacleType PullObstacle()
        {
            if (_obstacleQueue.Count == 0)
                return default;

            ObstacleType spawn = _obstacleQueue.Dequeue();
            _activeObstacles.Add(spawn);

            if (OnActiveObstaclesChange != null)
                OnActiveObstaclesChange();

            return spawn;
        }

        public ObstacleType[] GenerateNewObstacles(int generate_amount)
        {
            if (generate_amount <= 0)
                return null;
            generate_amount = Mathf.Clamp(generate_amount, 0, 100);

            ObstacleType[] generated_obstacles = GenerateObstacles(generate_amount);
            _allObstacles.AddRange(generated_obstacles);
            return generated_obstacles;
        }

        private ObstacleType[] GenerateObstacles(int amount)
        {
            ObstacleType generatedObstacle;
            ObstacleType[] generatedObstacles = new ObstacleType[amount];

            for (int i = 0; i < amount; i++)
            {
                generatedObstacle = GameObject.Instantiate(Data.ObstaclePrefab).GetComponent<ObstacleType>();
                generatedObstacle.Generate(Data, _controller);

                generatedObstacles[i] = generatedObstacle;
                _obstacleQueue.Enqueue(generatedObstacle);
            }
            return generatedObstacles;
        }

        public readonly ObstacleScriptableObject Data;
        private readonly IObstacleController _controller;

        private readonly Queue<ObstacleType> _obstacleQueue;
        private readonly List<ObstacleType> _activeObstacles;
        private readonly List<ObstacleType> _allObstacles;

        public int ObstaclesInPool => _obstacleQueue.Count;
        public ObstacleType[] ActiveObstacles => _activeObstacles.ToArray();
        public ObstacleType[] AllObstacles => _allObstacles.ToArray();

        public ObstaclePool(ObstacleScriptableObject data, IObstacleController controller, int generate_amount)
        {
            Data = data;
            _controller = controller;
            _obstacleQueue = new(generate_amount);

            _activeObstacles = new(generate_amount);

            _allObstacles = new();
            _allObstacles.AddRange(GenerateObstacles(generate_amount));
        }
    }
}