using UnityEngine;
using UnityEditor;

using JumpMaster.CameraControls;

namespace JumpMaster.Editors
{
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor
    {

        private SerializedProperty _cameraData;

        private void OnEnable()
        {
            _cameraData = serializedObject.FindProperty("_data");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_cameraData, new GUIContent("Camera Data"));
            Editor camData = CreateEditor(_cameraData.objectReferenceValue);
            camData.OnInspectorGUI();
        }
    }
}