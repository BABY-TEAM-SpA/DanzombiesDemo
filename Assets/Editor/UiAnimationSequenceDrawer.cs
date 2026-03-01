#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UiAnimationSequence))]
public class UiAnimationSequenceDrawer : PropertyDrawer
{
    const float SPACING = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        float line = EditorGUIUtility.singleLineHeight;

        var loop = property.FindPropertyRelative("loop");
        var loopCount = property.FindPropertyRelative("loopCount");
        var onStart = property.FindPropertyRelative("OnSequenceStart");
        var onComplete = property.FindPropertyRelative("OnSequenceComplete");
        var channels = property.FindPropertyRelative("animationChannels");

        // sequenceName
        height += line + SPACING;

        // loop
        height += line + SPACING;

        // loopCount (solo si loop activo)
        if (loop.boolValue)
            height += line + SPACING;

        // OnSequenceStart
        height += EditorGUI.GetPropertyHeight(onStart, true) + SPACING;

        // OnSequenceComplete
        height += EditorGUI.GetPropertyHeight(onComplete, true) + SPACING;

        // animationChannels (array expandible)
        height += EditorGUI.GetPropertyHeight(channels, true) + SPACING;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float line = EditorGUIUtility.singleLineHeight;
        float y = position.y;

        var sequenceName = property.FindPropertyRelative("sequenceName");
        var loop = property.FindPropertyRelative("loop");
        var loopCount = property.FindPropertyRelative("loopCount");
        var onStart = property.FindPropertyRelative("OnSequenceStart");
        var onComplete = property.FindPropertyRelative("OnSequenceComplete");
        var channels = property.FindPropertyRelative("animationChannels");

        Rect rect = new Rect(position.x, y, position.width, line);

        // sequenceName
        EditorGUI.PropertyField(rect, sequenceName);
        y += line + SPACING;

        // loop
        rect.y = y;
        EditorGUI.PropertyField(rect, loop);
        y += line + SPACING;

        // loopCount condicional
        if (loop.boolValue)
        {
            rect.y = y;
            EditorGUI.PropertyField(rect, loopCount);
            y += line + SPACING;
        }

        // OnSequenceStart
        rect.y = y;
        rect.height = EditorGUI.GetPropertyHeight(onStart, true);
        EditorGUI.PropertyField(rect, onStart);
        y += rect.height + SPACING;

        // OnSequenceComplete
        rect.y = y;
        rect.height = EditorGUI.GetPropertyHeight(onComplete, true);
        EditorGUI.PropertyField(rect, onComplete);
        y += rect.height + SPACING;

        // animationChannels
        rect.y = y;
        rect.height = EditorGUI.GetPropertyHeight(channels, true);
        EditorGUI.PropertyField(rect, channels, true);

        EditorGUI.EndProperty();
    }
}
#endif