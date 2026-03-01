#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UiAnimation.AnimationStep))]
public class AnimationStepDrawer : PropertyDrawer
{
    const float SPACING = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + SPACING; // stepType
        height += EditorGUIUtility.singleLineHeight + SPACING;      // duration

        var type = property.FindPropertyRelative("stepType");
        var stepType =
            (UiAnimation.AnimationStep.UiStepType)type.enumValueIndex;

        if (stepType != UiAnimation.AnimationStep.UiStepType.Wait)
            height += EditorGUIUtility.singleLineHeight + SPACING;  // easing

        switch (stepType)
        {
            case UiAnimation.AnimationStep.UiStepType.Move:
            case UiAnimation.AnimationStep.UiStepType.MoveTo:
                height += EditorGUIUtility.singleLineHeight + SPACING;
                break;

            case UiAnimation.AnimationStep.UiStepType.Scale:
                height += EditorGUIUtility.singleLineHeight + SPACING;
                break;

            case UiAnimation.AnimationStep.UiStepType.Rotate:
                height += EditorGUIUtility.singleLineHeight + SPACING;
                break;

            case UiAnimation.AnimationStep.UiStepType.Fade:
                height += EditorGUIUtility.singleLineHeight + SPACING;
                break;

            case UiAnimation.AnimationStep.UiStepType.Color:
                height += EditorGUIUtility.singleLineHeight + SPACING;
                break;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float y = position.y;

        var type = property.FindPropertyRelative("stepType");
        var duration = property.FindPropertyRelative("duration");
        var easing = property.FindPropertyRelative("easing");

        var positionProp = property.FindPropertyRelative("position");
        var scaleProp = property.FindPropertyRelative("scale");
        var rotationProp = property.FindPropertyRelative("rotation");
        var alphaProp = property.FindPropertyRelative("alpha");
        var colorProp = property.FindPropertyRelative("color");

        Rect rect = new Rect(position.x, y, position.width, lineHeight);
        EditorGUI.PropertyField(rect, type);
        y += lineHeight + SPACING;

        rect.y = y;
        EditorGUI.PropertyField(rect, duration);
        y += lineHeight + SPACING;

        var stepType =
            (UiAnimation.AnimationStep.UiStepType)type.enumValueIndex;

        if (stepType != UiAnimation.AnimationStep.UiStepType.Wait)
        {
            rect.y = y;
            EditorGUI.PropertyField(rect, easing);
            y += lineHeight + SPACING;
        }

        rect.y = y;

        switch (stepType)
        {
            case UiAnimation.AnimationStep.UiStepType.Move:
            case UiAnimation.AnimationStep.UiStepType.MoveTo:
                EditorGUI.PropertyField(rect, positionProp);
                break;

            case UiAnimation.AnimationStep.UiStepType.Scale:
                EditorGUI.PropertyField(rect, scaleProp);
                break;

            case UiAnimation.AnimationStep.UiStepType.Rotate:
                EditorGUI.PropertyField(rect, rotationProp);
                break;

            case UiAnimation.AnimationStep.UiStepType.Fade:
                EditorGUI.PropertyField(rect, alphaProp);
                break;

            case UiAnimation.AnimationStep.UiStepType.Color:
                EditorGUI.PropertyField(rect, colorProp);
                break;
        }

        EditorGUI.EndProperty();
    }
}
#endif