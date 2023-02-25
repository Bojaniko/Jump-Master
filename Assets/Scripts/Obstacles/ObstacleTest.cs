using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.Obstacles.Test
{
    [DefaultExecutionOrder(-1)]
    public class ObstacleTest : MonoBehaviour
    {

        public GameObject Obstacle;

        public SpawnSO SpawnData;

        private void Awake()
        {
            if (!enabled)
                return;

            /*if (Obstacle != null)
            {
                if (Obstacle.GetComponent<Obstacle>() != null)
                {
                    Obstacle.GetComponent<Obstacle>().OnInitialized += SpawnTestObstacle;
                }
            }*/
        }

        private void SpawnTestObstacle()
        {
            /*if (SpawnData != null)
            {
                if (SpawnData.GetType().Equals(typeof(LaserGateSpawnSO)))
                {
                    ISpawnable<LaserGateSpawnSO> gate_spawnable = Obstacle.GetComponent<Obstacle>() as ISpawnable<LaserGateSpawnSO>;

                    gate_spawnable.Spawn(SpawnData as LaserGateSpawnSO, Obstacle.transform.position);
                }

                if (SpawnData.GetType().Equals(typeof(MissileSpawnSO)))
                {
                    ISpawnable<MissileSpawnSO> missile_spawnable = Obstacle.GetComponent<Obstacle>() as ISpawnable<MissileSpawnSO>;

                    missile_spawnable.Spawn(SpawnData as MissileSpawnSO, Obstacle.transform.position);

                    //missile_spawnable.OnDespawnable += missile_spawnable.Despawn;
                }
            }*/
        }
    }
}