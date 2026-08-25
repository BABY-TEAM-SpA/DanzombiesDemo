using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DanceZone), true)]
public class DanceZoneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;

        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;

            EditorGUILayout.PropertyField(property, true);

            if (property.name == "dancers")
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Refresh"))
                {
                    DanceZone danceZone = (DanceZone)target;

                    Undo.RecordObject(danceZone, "Refresh DanceZone Zombies");
                    danceZone.RefreshZombies();
                    EditorUtility.SetDirty(danceZone);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
