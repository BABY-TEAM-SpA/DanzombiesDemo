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
     [Tooltip("Pasos en 1 Beat (recomendable 1). Si se ponen 2 hará como Corcheas y si se ponen 3 hará Trecillos")]
     public List<DanceStep> StepPerBeat = new List<DanceStep>();
 }

[Serializable]
 public class DancePattern
 {
     
     [Tooltip("Coreo por Beat (recomendable 4)")]
     public List<DanceStepPerBeat> StepInBar = new List<DanceStepPerBeat>();
 }

 public class DanceSequence : MonoBehaviour
 {
     public enum SeqStopMode
     {
         OneShot,
         StopOnCombo,
         StopOnFullFlow,
         Loop,
         LoopShuffled
     }
     public SeqStopMode sequenceStopMode = SeqStopMode.OneShot;
     public DancePattern coreography;
     
     public UnityEvent OnDanceSequenceFinished = new UnityEvent();

     public DanceStep GetDanceStep(int beat, BeatManager.BeatType beatPart)
     {
         DanceStep step = DanceStep.None;
         if (coreography.StepInBar.Count > 0 && beat >= 0)
         {
             DanceStepPerBeat beatDances = coreography.StepInBar[beat % coreography.StepInBar.Count];
             if (beatPart == BeatManager.BeatType.FullBeat)
             {
                 step = beatDances.StepPerBeat.Count!=0?beatDances.StepPerBeat[0]:DanceStep.None;
                 if (step == DanceStep.None) step = DanceStep.Idle;
             }
             else step = (beatDances.StepPerBeat.Count==2)? beatDances.StepPerBeat[1] : DanceStep.None;
             
         }
         return step;
     }
     

     //Aqui esta el problema del looping
     public DanceStep GetFutureStep(int nextStepCounter)
     {
         //if(nextStepCounter >= coreography.StepInBar.Count) EndSequence();
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
     
     public bool CheckEndOfSequence(int beat)
     {
         if(beat == coreography.StepInBar.Count)
         {
             if(sequenceStopMode == SeqStopMode.LoopShuffled)
             {
                 ShuffleSteps();
                 return false;
             }
             if(sequenceStopMode == SeqStopMode.OneShot)
             {
                 return true;
             }
         }
         return false;
     }

     

 }