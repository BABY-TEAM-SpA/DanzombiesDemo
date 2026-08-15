using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum DanceStep
{
    None,
    L_North,
    R_North,
    L_South,
    R_South,
    L_West,
    R_West,
    L_East,
    R_East,
    Idle
}


[Serializable]
public class DanceStepPerBeat
 {
     [Description("Pasos en 1 Beat (recomendable 1)")]
     public class MaxElementsAttribute : PropertyAttribute
     {
         public int Max { get; private set; }
         public MaxElementsAttribute(int max) { Max = max; }
     }
     #if UNITY_EDITOR
     
         [CustomPropertyDrawer(typeof(MaxElementsAttribute))]
         public class MaxElementsDrawer : PropertyDrawer
         {
             public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
             {
                 MaxElementsAttribute maxAttribute = (MaxElementsAttribute)attribute;
     
                 if (property.isArray)
                 {
                     if (property.arraySize > maxAttribute.Max)
                     {
                         property.arraySize = maxAttribute.Max;
                         Debug.LogWarning($"[Límite] No puedes añadir más de {maxAttribute.Max} subdivisiones aquí.");
                     }
                 }
                 EditorGUI.PropertyField(position, property, label, true);
             }
     
             public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
             {
                 return EditorGUI.GetPropertyHeight(property, label, true);
             }
         }
     #endif
     
     [MaxElements(2)]
     public List<DanceStep> StepPerBeat = new List<DanceStep>();
 }

[Serializable]
 public class DancePattern
 {
     
     [Description("Coreo por Beat (recomendable 4)")]
     public List<DanceStepPerBeat> StepInBar = new List<DanceStepPerBeat>();
 }

 public class DanceSequence : MonoBehaviour
 {
     public enum SeqMode
     {
         OneShot,
         StopOnCombo,
         StopOnFullFlow,
         Loop,
         LoopShuffled
     }
     public SeqMode sequenceType=SeqMode.OneShot;
     public DancePattern coreography;
     public delegate void ResetPointReached();
     public event ResetPointReached OnResetPointReached;
     public UnityEvent OnDanceSequenceFinished = new UnityEvent();

     public DanceStep GetDanceStep(int beat, BeatManager.BeatType beatPart)
     {
         DanceStep step = DanceStep.None;
         if (coreography.StepInBar.Count > 0 && beat >= 0)
         {
             DanceStepPerBeat beatDances = coreography.StepInBar[beat % coreography.StepInBar.Count];
             if (beatPart == BeatManager.BeatType.FullBeat)
             {
                 step = beatDances.StepPerBeat[0];
                 if (step == DanceStep.None) step = DanceStep.Idle;
             }
             else step = (beatDances.StepPerBeat.Count==2)? beatDances.StepPerBeat[1] : DanceStep.None;
             
         }
         return step;
     }

     public DanceStep GetFutureStep(int nextStepCounter)
     {
         if(nextStepCounter >= coreography.StepInBar.Count) EndSequence();
         /*for (int i = 0; i < coreography.StepInBar.Count; i++)
         {
             int aux = i+coreography;
             aux = aux % coreography.Count;
             if (coreography[aux] != DanceStep.None)
                 return coreography[aux];
         }*/
         return DanceStep.None;
     }
     
     public void ShuffleSteps()
     {
         coreography.StepInBar = coreography.StepInBar.OrderBy(x => UnityEngine.Random.value).ToList();
     }

     private void EndSequence()
     {
         OnResetPointReached?.Invoke();
         if(sequenceType == SeqMode.LoopShuffled) ShuffleSteps();
         if(sequenceType == SeqMode.OneShot) CompleteSequence();
     }
     public void CompleteSequence()
     {
         OnDanceSequenceFinished?.Invoke();
     }

 }