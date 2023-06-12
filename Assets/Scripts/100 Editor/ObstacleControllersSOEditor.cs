using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace JumpMaster.Obstacles
{
    [CustomEditor(typeof(ObstacleControllersSO))]
    public class ObstacleControllersSOEditor : Editor
    {
        private List<ScriptableObject> _tempSelect;

        private ObstacleControllersSO _target;

        private GUIContent[] _popups;
        private int _popupSelection;

        private Object _selectedData;

        private void OnEnable()
        {
            _target = target as ObstacleControllersSO;

            _tempSelect = new();
        }

        private void ResetPopup()
        {
            _popups = new GUIContent[_target.SpawnMetricsData.Length];
            for (int i = 0; i < _target.SpawnMetricsData.Length; i++)
            {
                _popups[i] = new(_target.SpawnMetricsData[i].ToString());
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.ObjectField(_selectedData, typeof(ISpawnMetricsSO), false, GUILayoutOption.);

            if (GUILayout.Button("Select Data"))
                EditorGUIUtility.ShowObjectPicker<SpawnMetricsSO<ObstacleSO, SpawnSO>>(_selectedData, false, "", 0);

            if (Event.current.commandName.Equals("ObjectSelectorUpdated"))
                _selectedData = EditorGUIUtility.GetObjectPickerObject();


            if (GUILayout.Button("Add Selected"))
                AddSelectedSpawnMetrics(_selectedData as ISpawnMetricsSO);

            if (_target.SpawnMetricsData.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                ResetPopup();
                _popupSelection = EditorGUILayout.Popup(_popupSelection, _popups);
                if (GUILayout.Button("Remove")) RemoveSelectedSpawnMetrics(_popupSelection);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Edit selected spawn metrics:");
                Editor _sme = CreateEditor(_target.SpawnMetricsData[_popupSelection]);
                _sme.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void AddSelectedSpawnMetrics(ISpawnMetricsSO spawn_metrics)
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
            if (!_tempSelect.Contains((ScriptableObject)spawn_metrics))
                _tempSelect.Add((ScriptableObject)spawn_metrics);
            _target.SpawnMetricsData = _tempSelect.ToArray();

            _popupSelection = _target.SpawnMetricsData.Length - 1;
        }

        private void RemoveSelectedSpawnMetrics(int index)
        {
            if (index >= _target.SpawnMetricsData.Length)
                return;

            _tempSelect.Clear();
            _tempSelect.AddRange(_target.SpawnMetricsData);
            if (_tempSelect.Contains(_target.SpawnMetricsData[index]))
                _tempSelect.Remove(_target.SpawnMetricsData[index]);
            _target.SpawnMetricsData = _tempSelect.ToArray();

            _popupSelection = _target.SpawnMetricsData.Length - 1;
        }
    }
}