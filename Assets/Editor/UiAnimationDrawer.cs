#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UiAnimation))]
public class UiAnimationDrawer : PropertyDrawer
{
    const float SPACING = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        float line = EditorGUIUtility.singleLineHeight;

        var revert = property.FindPropertyRelative("revertOnComplete");
        var target = property.FindPropertyRelative("objectToAnimate");
        var timeMode = property.FindPropertyRelative("timeMode");
        var steps = property.FindPropertyRelative("animationSteps");

        // revertOnComplete
        height += line + SPACING;

        // objectToAnimate
        height += line + SPACING;

        // timeMode
        height += line + SPACING;

        // animationSteps (array expandible)
        height += EditorGUI.GetPropertyHeight(steps, true) + SPACING;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float line = EditorGUIUtility.singleLineHeight;
        float y = position.y;

        var revert = property.FindPropertyRelative("revertOnComplete");
        var target = property.FindPropertyRelative("objectToAnimate");
        var timeMode = property.FindPropertyRelative("timeMode");
        var steps = property.FindPropertyRelative("animationSteps");

        Rect rect = new Rect(position.x, y, position.width, line);

        // revertOnComplete
        EditorGUI.PropertyField(rect, revert);
        y += line + SPACING;

        // objectToAnimate
        rect.y = y;
        EditorGUI.PropertyField(rect, target);
        y += line + SPACING;

        // timeMode
        rect.y = y;
        EditorGUI.PropertyField(rect, timeMode);
        y += line + SPACING;

        // animationSteps
        rect.y = y;
        rect.height = EditorGUI.GetPropertyHeight(steps, true);
        EditorGUI.PropertyField(rect, steps, true);

        EditorGUI.EndProperty();
    }
}
#endif