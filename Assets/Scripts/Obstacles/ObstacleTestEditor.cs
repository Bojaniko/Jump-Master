using UnityEngine;
using UnityEditor;

using JumpMaster.Obstacles;

namespace JumpMaster.Obstacles.Test
{
    [CustomEditor(typeof(ObstacleTest))]
    public class ObstacleTestEditor : Editor
    {
        /*public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ObstacleTest tester = target as ObstacleTest;

            bool spawned = false;

            if (tester.SpawnData.GetType().Equals(typeof(LaserGateSpawnSO)))
            {
                ISpawnable<LaserGateSpawnSO> spawnableLaserGate = tester.Obstacle.GetComponent<LaserGate>() as ISpawnable<LaserGateSpawnSO>;

                spawned = spawnableLaserGate.Spawned;
            }

            if (!spawned)
            {
                if (GUILayout.Button("Spawn"))
                {

                }
            }
            else
            {

            }
        }

        private void SpawnableGUI<SpawnType>()
        {

        }*/
    }
}