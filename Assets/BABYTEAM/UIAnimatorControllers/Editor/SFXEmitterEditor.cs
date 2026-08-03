using FMODUnity;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SFXEmitter))]
public class SFXEmitterEditor : Editor
{
    private bool showParams = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SFXEmitter emitter = (SFXEmitter)target;
        EditorGUILayout.Space();

        EventReference eventRef = emitter.eventRef;
        if (eventRef.IsNull)
            return;

        EditorEventRef editorEventRef = EventManager.EventFromGUID(eventRef.Guid);
        if (editorEventRef == null)
        {
            Debug.LogWarning($"[SFXEmitterEditor] No se encontró información del evento '{eventRef.Path}'." +
                $"Refrescar los FMOD Banks podría solucionarlo.", this);
            return;
        }

        showParams = EditorGUILayout.BeginFoldoutHeaderGroup(showParams, "Parameters");
        if (!showParams)
            return;

        EditorGUI.indentLevel++;

        if (editorEventRef.Parameters == null || editorEventRef.Parameters.Count == 0)
            EditorGUILayout.HelpBox($"El evento '{editorEventRef.Path}' no tiene parámetros locales configurados en FMOD Studio.", MessageType.Info);
        else
        {
            foreach (EditorParamRef param in editorEventRef.Parameters)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField(param.Name, EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                if (param.Type == ParameterType.Labeled)
                {
                    string defaultLabel = (param.Labels != null && (int)param.Default >= 0 && (int)param.Default < param.Labels.Length)
                        ? param.Labels[(int)param.Default]
                        : param.Default.ToString();

                    string labelsList = param.Labels != null
                        ? string.Join(", ", param.Labels.Select((label, index) => $"{index}: {label}"))
                        : "";

                    EditorGUILayout.LabelField($"{labelsList}   |   Default={param.Default}");
                }
                else EditorGUILayout.LabelField($"{param.Min}-{param.Max}   |   Default={param.Default}");

                if (GUILayout.Button("Use", EditorStyles.miniButton))
                    SetParameter(param);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void SetParameter(EditorParamRef param)
    {
        SFXEmitter emitter = (SFXEmitter)target;

        Undo.RecordObject(emitter, "Set Active FMOD Parameter");
        ParamRef activeParam = new ParamRef();
        activeParam.Name = param.Name;
        activeParam.Value = param.Default;
        activeParam.ID = param.ID;
        emitter.SetParameter(activeParam);

        EditorUtility.SetDirty(emitter);
    }
}
