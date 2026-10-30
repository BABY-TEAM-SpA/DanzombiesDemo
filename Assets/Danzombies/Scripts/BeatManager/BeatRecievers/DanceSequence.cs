using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DanceStep
{
    None,
    L_North, R_North,
    L_South, R_South,
    L_West, R_West,
    L_East, R_East
}


[Serializable]
public class DanceStepPerBeat
{
    [Tooltip("Pasos en 1 Beat (recomendable 1). Si se ponen 2 hará como Corcheas y si se ponen 3 hará Trecillos.")]
    public List<DanceStep> StepPerBeat = new List<DanceStep>();
}

[Serializable]
public class DancePattern
{
    [Tooltip("Coreo por Beat (recomendable 4).")]
    public List<DanceStepPerBeat> StepInBar = new List<DanceStepPerBeat>();
}

public class DanceSequence : MonoBehaviour
{
    #region [VARIABLES]
    public enum SeqStopMode
    {
        OneShot,
        StopOnComboC, StopOnComboB, StopOnComboA, StopOnComboS,
        StopOnFullFlow,
        Loop, LoopShuffled,
        StopOnXCorrectSteps
    }

    public SeqStopMode sequenceStopMode = SeqStopMode.OneShot;
    public CorrectSteps correctSteps;
    private int correctStepsCount = 0;

    [Header("")]
    public DancePattern coreography;

    [Header("")]
    [SerializeField] private UnityEvent OnDanceSequenceStarted = new UnityEvent();     
    public UnityEvent OnDanceSequenceFinished = new UnityEvent();

    [Serializable]
    public class CorrectSteps
    {
        [Min(1)] public int amount = 1; // <- N° de steps correctos

        [Tooltip("Feedbacks considerados como correctos.")]
        public BeatReciever.BeatFeedback[] feedbacks;
    }
    #endregion

    #region [METHODS]
    public void ActivateSequence() => OnDanceSequenceStarted?.Invoke();
    
    public void StopSequence()
    {
        correctStepsCount = 0;
        OnDanceSequenceFinished?.Invoke();
    }

    public bool CheckEndOfSequence(int beat)
    {
        switch (sequenceStopMode)
        {
            case SeqStopMode.OneShot:
                if (beat != coreography.StepInBar.Count) return false;
                StopSequence();
                return true;

            case SeqStopMode.StopOnComboC:
            case SeqStopMode.StopOnComboB:
            case SeqStopMode.StopOnComboA:
            case SeqStopMode.StopOnComboS:
                bool isSameCombo = false;
                if (PlayerManager.Player)
                    isSameCombo = PlayerManager.Player.ComboCtrl.State.ToString()
                        == sequenceStopMode.ToString().Substring(sequenceStopMode.ToString().Length - 1);
                if (isSameCombo) StopSequence();
                return isSameCombo;

            case SeqStopMode.StopOnFullFlow:
                bool isBarFilled = false;
                if (DanceBarController.Instance)
                    isBarFilled = DanceBarController.Instance.IsFilled;
                if (isBarFilled)
                    StopSequence();
                return isBarFilled;

            case SeqStopMode.Loop:
                return false;

            case SeqStopMode.LoopShuffled:
                if (beat != coreography.StepInBar.Count)
                    return false;
                ShuffleSteps();
                return false;

            case SeqStopMode.StopOnXCorrectSteps:
                if (sequenceStopMode != SeqStopMode.StopOnXCorrectSteps)
                    return false;
                bool hasReached = correctStepsCount >= correctSteps.amount;
                if (hasReached)
                    StopSequence();
                return hasReached;
                
            default:
                return false;
        }
    }

    #region Helpers
    public void ShuffleSteps() => coreography.StepInBar = coreography.StepInBar.OrderBy(x => UnityEngine.Random.value).ToList();

    public void RegisterStepFeedback(BeatReciever.BeatFeedback feedback)
    {
        if (sequenceStopMode != SeqStopMode.StopOnXCorrectSteps)
            return;
        if (correctSteps.feedbacks != null && correctSteps.feedbacks.Contains(feedback))
            correctStepsCount++;
    }
    public void ResetProgress() => correctStepsCount = 0;
    #endregion

    #region Get Steps
    /** beatCount goes from 1 to upper or +inf
     * beatPart goes from 0 to 5
     */
    public DanceStep GetDanceStep(int beatCount=1, BeatManager.BeatType beatPart=BeatManager.BeatType.FullBeat)
    {
        beatCount -= 1;
        if (coreography.StepInBar.Count == 0) return DanceStep.None; 
        if (beatCount < 0) return DanceStep.None;
        
        int index = beatCount % coreography.StepInBar.Count;
        DanceStepPerBeat beatDances = coreography.StepInBar[index];
        if(beatDances.StepPerBeat.Count==0) return DanceStep.None;
        
        switch (beatPart)
        {
            case BeatManager.BeatType.FullBeat: //(index,0)
                return(beatDances.StepPerBeat.Count >= 1)?beatDances.StepPerBeat[0]: DanceStep.None;
            case BeatManager.BeatType.FirstThird: //(index,2)
                return (beatDances.StepPerBeat.Count == 3) ? beatDances.StepPerBeat[1] : DanceStep.None;
            case BeatManager.BeatType.HalfBeat: //(index,3)
                return (beatDances.StepPerBeat.Count == 2) ? beatDances.StepPerBeat[1] : DanceStep.None;
            case BeatManager.BeatType.SecondThird: //(index,4)
                return (beatDances.StepPerBeat.Count == 3) ? beatDances.StepPerBeat[2] : DanceStep.None;
            default:
                return DanceStep.None;
        }
        
    }
    public DanceStep GetFutureStep(int currentBeat, BeatManager.BeatType beatPart)
    {
        int nextStepCounter = (currentBeat + 1) % coreography.StepInBar.Count;
        return coreography.StepInBar[nextStepCounter].StepPerBeat[0];
    }
    #endregion
    #endregion
}