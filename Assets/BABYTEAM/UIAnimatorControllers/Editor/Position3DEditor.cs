using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Position3D))]
public class Position3DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Position3D position3D = (Position3D)target;
        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh Renderers"))
        {
            Undo.RecordObject(position3D, "Refresh Position3D Renderers");
            position3D.RefreshRenderers();
            EditorUtility.SetDirty(position3D);
        }
    }
}
