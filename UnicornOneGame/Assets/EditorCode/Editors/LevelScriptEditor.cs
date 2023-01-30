using System.Collections;
using System.Collections.Generic;
using UnicornOne.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace UnicornOneEitor
{
    [CustomEditor(typeof(LevelScript))]
    public class LevelScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.LinkButton("Click me");
        }
    }
}
