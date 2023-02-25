using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace JumpMaster.LevelControllers.Obstacles
{
    [CustomEditor(typeof(ObstacleControllersSO))]
    public class ObstacleControllersSOEditor : Editor
    {
        private List<ObstacleControllerType> _tempSelection;

        private ObstacleControllersSO _target;

        private SerializedProperty _selectedObstacle;

        private SerializedProperty _fallingBomb;
        private SerializedProperty _laserGate;
        private SerializedProperty _missile;

        private void OnEnable()
        {
            _target = target as ObstacleControllersSO;

            _selectedObstacle = serializedObject.FindProperty("SelectedObstacle");

            _fallingBomb = serializedObject.FindProperty("FallingBomb");
            _laserGate = serializedObject.FindProperty("LaserGate");
            _missile = serializedObject.FindProperty("Missile");

            _tempSelection = new();

            if (_target.SelectedObstacles == null)
                _target.SelectedObstacles = new ObstacleControllerType[0];
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_selectedObstacle);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Obstacle"))
            {
                if (_selectedObstacle != null)
                    AddSelectedObstacleType((ObstacleControllerType)_selectedObstacle.enumValueIndex);
            }
            if (GUILayout.Button("Remove Obstacle"))
            {
                if (_selectedObstacle != null)
                    RemoveSelectedObstacleType((ObstacleControllerType)_selectedObstacle.enumValueIndex);
            }
            EditorGUILayout.EndHorizontal();

            if (_target.SelectedObstacles.Length > 0)
                EditorGUILayout.LabelField("Obstacle Spawn Metrics");

            for (int i = 0; i < _target.SelectedObstacles.Length; i++)
            {
                switch (_target.SelectedObstacles[i])
                {
                    case ObstacleControllerType.FallingBomb:
                        EditorGUILayout.PropertyField(_fallingBomb);
                        break;

                    case ObstacleControllerType.LaserGate:
                        EditorGUILayout.PropertyField(_laserGate);
                        break;

                    case ObstacleControllerType.Missile:
                        EditorGUILayout.PropertyField(_missile);
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void AddSelectedObstacleType(ObstacleControllerType controller_type)
        {
            _tempSelection.Clear();
            if (_target.SelectedObstacles.Length > 0)
                _tempSelection.AddRange(_target.SelectedObstacles);
            if (_tempSelection.Contains(controller_type) == false)
                _tempSelection.Add(controller_type);
            _target.SelectedObstacles = _tempSelection.ToArray();
        }

        private void RemoveSelectedObstacleType(ObstacleControllerType controller_type)
        {
            if (_target.SelectedObstacles.Length == 0)
                return;
            _tempSelection.Clear();
            _tempSelection.AddRange(_target.SelectedObstacles);
            if (_tempSelection.Contains(controller_type))
                _tempSelection.Remove(controller_type);
            _target.SelectedObstacles = _tempSelection.ToArray();

            switch (controller_type)
            {
                case ObstacleControllerType.FallingBomb:
                    _target.FallingBomb = null;
                    break;

                case ObstacleControllerType.LaserGate:
                    _target.LaserGate = null;
                    break;

                case ObstacleControllerType.Missile:
                    _target.Missile = null;
                    break;
            }
        }
    }
}