using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UiAnimatorController))]
public class UiAnimatorControllerEditor : Editor
{
    private UiAnimatorController controller;
    private SerializedProperty targetProp;
    private SerializedProperty playOnEnableProp;
    private SerializedProperty sequenceProp;

    private bool showSequence = true;
    private bool showSteps = true;

    private void OnEnable()
    {
        controller = (UiAnimatorController)target;
        targetProp = serializedObject.FindProperty("target");
        playOnEnableProp = serializedObject.FindProperty("playOnEnable");
        sequenceProp = serializedObject.FindProperty("sequence");
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("UI Animator Controller", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(playOnEnableProp);

        EditorGUILayout.Space(8);
        DrawSequence(sequenceProp);

        EditorGUILayout.Space(10);
        DrawRuntimeControls();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSequence(SerializedProperty seqProp)
    {
        if (seqProp == null) return;

        EditorGUILayout.BeginVertical("box");
        showSequence = EditorGUILayout.Foldout(showSequence, "Sequence", true);
        if (showSequence)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(seqProp.FindPropertyRelative("loop"));
            EditorGUILayout.PropertyField(seqProp.FindPropertyRelative("loopCount"));
            EditorGUILayout.PropertyField(seqProp.FindPropertyRelative("onSequenceStart"));

            var stepsProp = seqProp.FindPropertyRelative("steps");
            EditorGUILayout.Space(8);

            showSteps = EditorGUILayout.Foldout(showSteps, $"Steps ({stepsProp.arraySize})", true);
            if (showSteps)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < stepsProp.arraySize; i++)
                {
                    var stepProp = stepsProp.GetArrayElementAtIndex(i);
                    DrawStep(stepProp, i, stepsProp);
                }

                EditorGUILayout.Space(6);
                if (GUILayout.Button("+ Add Step"))
                    stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawStep(SerializedProperty stepProp, int index, SerializedProperty stepsProp)
    {
        var typeProp = stepProp.FindPropertyRelative("stepType");
        var durationProp = stepProp.FindPropertyRelative("duration");
        var animsProp = stepProp.FindPropertyRelative("animations");
        var completeEventProp = stepProp.FindPropertyRelative("onStepComplete");

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();

        stepProp.isExpanded = EditorGUILayout.Foldout(stepProp.isExpanded, $"Step {index + 1}", true);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("✕", GUILayout.Width(25)))
        {
            stepsProp.DeleteArrayElementAtIndex(index);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        if (stepProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(typeProp);
            var type = (UiAnimationSequence.SequenceStep.StepType)typeProp.enumValueIndex;

            switch (type)
            {
                case UiAnimationSequence.SequenceStep.StepType.Wait:
                    EditorGUILayout.PropertyField(durationProp);
                    EditorGUILayout.PropertyField(completeEventProp);
                    break;

                case UiAnimationSequence.SequenceStep.StepType.Animation:
                    EditorGUILayout.PropertyField(durationProp);
                    EditorGUILayout.PropertyField(animsProp);
                    EditorGUILayout.PropertyField(completeEventProp);
                    break;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawRuntimeControls()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);

        GUI.enabled = Application.isPlaying;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("▶ Play Sequence", GUILayout.Height(25)))
            controller.PlayAll();

        if (GUILayout.Button("⏹ Stop", GUILayout.Height(25)))
            controller.StopAll();
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
    }
}
