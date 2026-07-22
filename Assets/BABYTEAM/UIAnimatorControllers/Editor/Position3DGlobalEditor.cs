using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Position3DGlobal))]
public class Position3DGlobalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Position3DGlobal position3D = (Position3DGlobal)target;
        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh"))
        {
            Undo.RecordObject(position3D, "Refresh Position3DGlobal Renderers");
            position3D.FindChildrenPosition3D();
            position3D.RefreshRenderers();
            EditorUtility.SetDirty(position3D);
        }
    }
}
