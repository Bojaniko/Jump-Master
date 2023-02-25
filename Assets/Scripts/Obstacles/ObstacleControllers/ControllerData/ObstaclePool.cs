using System.Collections.Generic;

using UnityEngine;

using JumpMaster.LevelControllers.Obstacles;

namespace JumpMaster.Obstacles
{
    public class ObstaclePool<ObstacleType, ObstacleScriptableObject, SpawnScriptableObject, SpawnArguments>
        where ObstacleType : Obstacle
        where SpawnScriptableObject : SpawnSO
        where ObstacleScriptableObject : ObstacleSO
        where SpawnArguments : SpawnArgs
    {
        public readonly ObstacleScriptableObject Data;
        private readonly ObstacleController _controller;

        private Queue<ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments>> _obstacleQueue;
        private List<ObstacleType> _activeObstacles;
        private List<ObstacleType> _allObstacles;

        public int ObstaclesInPool
        {
            get
            {
                return _obstacleQueue.Count;
            }
        }
        public ObstacleType[] ActiveObstacles
        {
            get
            {
                return _activeObstacles.ToArray();
            }
        }
        public ObstacleType[] AllObstacles
        {
            get
            {
                return _allObstacles.ToArray();
            }
        }

        public void GenerateNewObstacles(int generate_amount)
        {
            _allObstacles.AddRange(GenerateObstacles(generate_amount));
        }

        public void SpawnObstacle(SpawnScriptableObject spawn_data, SpawnArguments spawn_args)
        {
            if (_obstacleQueue.Count > 0)
            {
                ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn = _obstacleQueue.Dequeue();
                _activeObstacles.Add(spawn as ObstacleType);
                spawn.SpawnController.Spawn(spawn_data, spawn_args);
            }
        }

        public void DespawnObstacle(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn)
        {
            if (_activeObstacles.Contains(spawn as ObstacleType))
            {
                if (_obstacleQueue.Contains(spawn) == false)
                {
                    spawn.SpawnController.Despawn();
                    _activeObstacles.Remove(spawn as ObstacleType);
                    _obstacleQueue.Enqueue(spawn);
                }
            }
        }

        public ObstaclePool(ObstacleScriptableObject data, ObstacleController controller, int generate_amount)
        {
            Data = data;
            _controller = controller;
            _obstacleQueue = new(generate_amount);

            _activeObstacles = new(generate_amount);

            _allObstacles = new();
            _allObstacles.AddRange(GenerateObstacles(generate_amount));
        }

        private ObstacleType[] GenerateObstacles(int amount)
        {
            if (amount < 1)
                amount = 1;

            ObstacleType generatedObstacle;

            ObstacleType[] generatedObstacles = new ObstacleType[amount];

            for (int i = 0; i < amount; i++)
            {
                generatedObstacle = GameObject.Instantiate(Data.ObstaclePrefab).GetComponent<Obstacle>() as ObstacleType;
                generatedObstacle.Generate(Data, _controller);

                generatedObstacles[i] = generatedObstacle;

                AddObstacle(generatedObstacle as ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments>);
            }
            return generatedObstacles;
        }

        private void AddObstacle(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn)
        {
            _obstacleQueue.Enqueue(spawn);
            spawn.SpawnController.OnDespawn += ObstacleDespawned;
        }

        private void ObstacleDespawned(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn)
        {
            if (_activeObstacles.Contains(spawn as ObstacleType))
            {
                _activeObstacles.Remove(spawn as ObstacleType);
                if (_obstacleQueue.Contains(spawn) == false)
                    _obstacleQueue.Enqueue(spawn);
            }
        }
    }
}