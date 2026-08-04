using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ZombieDanceZone), true)]
public class ZombieDanceZoneEditor : Editor
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

            if (property.name == "zombies")
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Refresh"))
                {
                    ZombieDanceZone zombieDanceZone = (ZombieDanceZone)target;

                    Undo.RecordObject(zombieDanceZone, "Refresh ZombieDanceZone Zombies");
                    zombieDanceZone.RefreshZombies();
                    EditorUtility.SetDirty(zombieDanceZone);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
