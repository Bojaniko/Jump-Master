using UnityEngine;
using UnityEditor;

using JumpMaster.Testing;

namespace JumpMaster.Obstacles.Test
{
    [CustomEditor(typeof(ObstacleTest))]
    public class ObstacleTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generat Obstacle"))
                ((ObstacleTest)serializedObject.targetObject).GenerateTestObstacle();

            if (GUILayout.Button("Spawn Obstacle"))
                ((ObstacleTest)serializedObject.targetObject).SpawnTestObstacle();
        }
    }
}