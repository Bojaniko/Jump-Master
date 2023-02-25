using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

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