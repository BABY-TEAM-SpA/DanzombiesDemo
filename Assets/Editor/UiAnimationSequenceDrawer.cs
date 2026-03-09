#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UiAnimationSequence))]
public class UiAnimationSequenceDrawer : PropertyDrawer
{
    const float SPACING = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;

        // Si está colapsado solo ocupa una línea
        if (!property.isExpanded)
            return line;

        float height = line + SPACING;

        var loop = property.FindPropertyRelative("loop");
        var loopCount = property.FindPropertyRelative("loopCount");
        var onStart = property.FindPropertyRelative("OnSequenceStart");
        var onComplete = property.FindPropertyRelative("OnSequenceComplete");
        var channels = property.FindPropertyRelative("animationChannels");

        height += line + SPACING;

        if (loop.boolValue)
            height += line + SPACING;

        height += EditorGUI.GetPropertyHeight(onStart, true) + SPACING;
        height += EditorGUI.GetPropertyHeight(onComplete, true) + SPACING;
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

        // Mostrar nombre de la secuencia en el foldout
        string title = string.IsNullOrEmpty(sequenceName.stringValue)
            ? property.displayName
            : sequenceName.stringValue;

        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, title, true);

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        y += line + SPACING;

        EditorGUI.indentLevel++;

        rect.y = y;
        rect.height = line;
        EditorGUI.PropertyField(rect, sequenceName);
        y += line + SPACING;

        rect.y = y;
        EditorGUI.PropertyField(rect, loop);
        y += line + SPACING;

        if (loop.boolValue)
        {
            rect.y = y;
            EditorGUI.PropertyField(rect, loopCount);
            y += line + SPACING;
        }

        rect.y = y;
        rect.height = EditorGUI.GetPropertyHeight(onStart, true);
        EditorGUI.PropertyField(rect, onStart);
        y += rect.height + SPACING;

        rect.y = y;
        rect.height = EditorGUI.GetPropertyHeight(onComplete, true);
        EditorGUI.PropertyField(rect, onComplete);
        y += rect.height + SPACING;

        rect.y = y;
        rect.height = EditorGUI.GetPropertyHeight(channels, true);
        EditorGUI.PropertyField(rect, channels, true);

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
#endif