#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(UiAnimator))]
public class UiAnimatorEditor : Editor
{
    private SerializedProperty sequencesProp;
    private ReorderableList sequenceList;

    private void OnEnable()
    {
        sequencesProp = serializedObject.FindProperty("sequences");

        sequenceList = new ReorderableList(
            serializedObject,
            sequencesProp,
            true, true, true, true);

        sequenceList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "UI Animation Sequences");
        };

        sequenceList.elementHeightCallback = index =>
        {
            var element = sequencesProp.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true) + 10;
        };

        sequenceList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = sequencesProp.GetArrayElementAtIndex(index);
            rect.y += 4;
            EditorGUI.PropertyField(rect, element, GUIContent.none, true);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        sequenceList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif