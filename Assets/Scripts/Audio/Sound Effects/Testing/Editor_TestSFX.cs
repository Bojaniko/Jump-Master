using UnityEngine;
using UnityEditor;

namespace Studio28.Audio.SFX.Testing
{
    [CustomEditor(typeof(TestSFX))]
    public class Editor_TestSFX : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TestSFX test = (TestSFX)serializedObject.targetObject;

            if (GUILayout.Button("Play sound")) test.PlaySound();

            if (GUILayout.Button("Stop sound")) test.StopSound();
        }
    }
}