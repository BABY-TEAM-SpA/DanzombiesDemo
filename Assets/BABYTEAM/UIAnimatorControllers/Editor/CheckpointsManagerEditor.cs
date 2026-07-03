using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckpointsManager))]
public class CheckpointsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CheckpointsManager checkpointsManager = (CheckpointsManager)target;
        EditorGUILayout.Space();

        if (GUILayout.Button("Collect Resettables"))
            checkpointsManager.CollectResettables();
    }
}
