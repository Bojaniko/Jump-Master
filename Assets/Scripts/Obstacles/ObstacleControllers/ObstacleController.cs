using UnityEngine;

namespace JumpMaster.LevelControllers.Obstacles
{
    public abstract class ObstacleController
    {
        public dynamic Self;
        public ObstacleController()
        {
            Self = this;
        }

        public int SpawnsLeft
        {
            get
            {
                if (Self.ControllerData.SpawnMetrics == null) return 0;
                return Self.ControllerData.SpawnMetrics.SpawnAmount - Self.ControllerData.ObstaclesSpawned;
            }
        }

        public void TrySpawn()
        {
            if (Self.ControllerData.SpawnMetrics == null)
                return;
            if (Self.ControllerData.ActiveObstacles.Length == Self.ControllerData.SpawnMetrics.MaxActiveObstacles)
                return;
            if (Self.ControllerData.ObstaclesSpawned >= Self.ControllerData.SpawnMetrics.SpawnAmount)
                return;
            //Debug.Log(Self.GetType() + " spawns left: " + SpawnsLeft);
            //Debug.Log(Self.GetType() + " max spawns: " + Self.ControllerData.SpawnMetrics.SpawnAmount);
            if (CanSpawn())
            {
                Spawn();
            }
        }
        protected abstract void Spawn();
        protected abstract bool CanSpawn();
    }
}