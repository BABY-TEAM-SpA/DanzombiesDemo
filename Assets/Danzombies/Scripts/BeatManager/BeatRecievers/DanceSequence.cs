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

    public bool CheckEndOfSequence(int beat)
    {
        switch (sequenceStopMode)
        {
            case SeqStopMode.OneShot:
                if (beat != coreography.StepInBar.Count)
                    return false;
                OnDanceSequenceFinished?.Invoke();
                return true;

            case SeqStopMode.StopOnComboC:
            case SeqStopMode.StopOnComboB:
            case SeqStopMode.StopOnComboA:
            case SeqStopMode.StopOnComboS:
                bool isSameCombo = PlayerManager.Player.ComboState.ToString() == sequenceStopMode
                    .ToString().Substring(sequenceStopMode.ToString().Length - 1);
                if (isSameCombo)
                    OnDanceSequenceFinished?.Invoke();
                return isSameCombo;

            case SeqStopMode.StopOnFullFlow:
                bool isBarFilled = DanceBarController.Instance.isBarFilled;
                if (isBarFilled)
                    OnDanceSequenceFinished?.Invoke();
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
                    OnDanceSequenceFinished?.Invoke();
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
    public DanceStep GetDanceStep(int beat, BeatManager.BeatType beatPart)
    {
        DanceStep step = DanceStep.None;
        if (coreography.StepInBar.Count > 0 && beat >= 0)
        {
            DanceStepPerBeat beatDances = coreography.StepInBar[beat % coreography.StepInBar.Count];
            if (beatPart == BeatManager.BeatType.FullBeat)
                step = beatDances.StepPerBeat.Count != 0 ? beatDances.StepPerBeat[0] : DanceStep.None;
            else step = (beatDances.StepPerBeat.Count == 2) ? beatDances.StepPerBeat[1] : DanceStep.None;
        }
        return step;
    }

    //Aqui esta el problema del looping
    public DanceStep GetFutureStep(int currentBeat, BeatManager.BeatType beatPart)
    {
        int nextStepCounter = (currentBeat + 1) % coreography.StepInBar.Count;
        return coreography.StepInBar[nextStepCounter].StepPerBeat[0];
    }
    #endregion
    #endregion
}