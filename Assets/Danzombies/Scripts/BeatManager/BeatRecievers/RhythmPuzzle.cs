using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;






public abstract class RhythmPuzzle : BeatReciever
{

    [Header("Rhythm Puzzle Settings")]
    [SerializeField] protected bool debug;
    [SerializeField] bool activateOnStart;
    protected DanceEventBuffer danceEventBuffer = new DanceEventBuffer();
    protected DanceSequence currentDanceSequence;
    protected int currentSequenceIndex;
    protected DanceStep currentStep;
    protected bool availableToDance = false;
    protected int innerBeatCounter = 0;
    
    
    protected void Start()
    {
        PreparePuzzle();
        if (activateOnStart) SetActivePuzzle(true);
    }
    private void OnDisable()
    {
        danceEventBuffer.RemoveAllListeners();
    }
    
    public abstract void PreparePuzzle();
    
    public virtual void SetActivePuzzle(bool activate)
    {
        isActive = activate;
        availableToDance = false;
        if(isActive)
        {
            if(debug)Debug.Log($"Starting PUZZLE ({name})");
            danceEventBuffer.InvokeEnablePuzzle(this);
        } else
        {
            if(debug)Debug.Log($"Stopping PUZZLE ({name})");
            danceEventBuffer.InvokeDisablePuzzle(this);
        }
        if (!activate) return;
        
    }
    

    public void SetSequence(DanceSequence sequence)
    {
        currentDanceSequence?.ResetProgress();
        currentDanceSequence = sequence;
    }
    public void GetDanceStep(int beat, BeatManager.BeatType beatType)
    {
        currentStep = currentDanceSequence.GetDanceStep(beat,beatType);
    }
    public void GetNextDanceStep(int beat, BeatManager.BeatType type)
    {
        DanceStep nextStep = currentDanceSequence.GetFutureStep(beat-1,type);
        danceEventBuffer.InvokeNextStep(beat,type,nextStep);
        if(debug) Debug.Log($"... Nextdance:{nextStep}");
    }

    public void ReactToFeedback(BeatReciever.BeatFeedback bf)
    {
        currentDanceSequence.RegisterStepFeedback(bf);
    }

    public override void PostBarAction(int beatCounter)
    {
        CheckEnd(beatCounter);
    }
    
    protected virtual void CheckEnd(int beat)
    {
        if(currentDanceSequence.CheckEndOfSequence(beat))OnPuzzleCompleted();
    }

    public void OnPuzzleCompleted()
    {
        //Debug.Log("Puzzle Is Over");
        SetActivePuzzle(false);
    }
    
    public void ActivatePuzzleByIndex(int index)
    {
        currentSequenceIndex = index;
        SetActivePuzzle(true);
    }
    

}
