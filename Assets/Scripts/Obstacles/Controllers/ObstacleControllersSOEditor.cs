using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace JumpMaster.Obstacles
{
    [CustomEditor(typeof(ObstacleControllersSO))]
    public class ObstacleControllersSOEditor : Editor
    {
        private List<SpawnMetricsBaseSO> _tempSelect;

        private ObstacleControllersSO _target;

        private GUIContent[] _popups;
        private SerializedProperty _popupSelection;

        private SerializedProperty _selectedSO;
        private SerializedProperty _spawnMetrics;

        private GUIContent _selectedSOLabel;

        private void OnEnable()
        {
            _target = target as ObstacleControllersSO;

            _selectedSO = serializedObject.FindProperty("SelectedSpawnMetrics");
            _spawnMetrics = serializedObject.FindProperty("SpawnMetricsData");

            _popupSelection = serializedObject.FindProperty("PopupSelection");

            _tempSelect = new();

            _selectedSOLabel = new("Selection", "Select obstacle spawn metrics data.");
        }

        private void ResetPopup()
        {
            _popups = new GUIContent[_spawnMetrics.arraySize];
            for (int i = 0; i < _spawnMetrics.arraySize; i++)
            {
                _popups[i] = new(_spawnMetrics.GetArrayElementAtIndex(i).objectReferenceValue.name);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Spawn Metrics");

            EditorGUILayout.PropertyField(_selectedSO, _selectedSOLabel);

            if (GUILayout.Button("Add"))
                AddSelectedSpawnMetrics(_selectedSO.objectReferenceValue as SpawnMetricsBaseSO);

            if (_spawnMetrics.arraySize > 0)
            {
                EditorGUILayout.BeginHorizontal();
                ResetPopup();
                _popupSelection.intValue = EditorGUILayout.Popup(_popupSelection.intValue, _popups);
                if (GUILayout.Button("Remove")) RemoveSelectedSpawnMetrics(_popupSelection.intValue);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Edit selected spawn metrics:");
                Editor _sme = CreateEditor(_spawnMetrics.GetArrayElementAtIndex(_popupSelection.intValue).objectReferenceValue);
                _sme.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void AddSelectedSpawnMetrics(SpawnMetricsBaseSO spawn_metrics)
        {
            if (spawn_metrics == null)
            {
                Debug.LogError("No spawn metrics selected");
                return;
            }

            if (!(spawn_metrics is ISpawnMetricsSO))
            {
                Debug.LogError("Selected scriptable object must be an obstacle spawn metric.");
                return;
            }

            _tempSelect.Clear();
            if (_target.SpawnMetricsData.Length > 0)
                _tempSelect.AddRange(_target.SpawnMetricsData);
            if (!_tempSelect.Contains(spawn_metrics))
                _tempSelect.Add(spawn_metrics);
            _target.SpawnMetricsData = _tempSelect.ToArray();
        }

        private void RemoveSelectedSpawnMetrics(int index)
        {
            if (index >= _spawnMetrics.arraySize)
                return;

            _tempSelect.Clear();
            _tempSelect.AddRange(_target.SpawnMetricsData);
            if (_tempSelect.Contains(_target.SpawnMetricsData[index]))
                _tempSelect.Remove(_target.SpawnMetricsData[index]);
            _target.SpawnMetricsData = _tempSelect.ToArray();
        }
    }
}