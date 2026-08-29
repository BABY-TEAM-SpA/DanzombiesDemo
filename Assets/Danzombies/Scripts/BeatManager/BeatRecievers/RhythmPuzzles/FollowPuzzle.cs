using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FollowDanceSequence
{
    public DanceSequence danceSequence;
    public List<Dancer> dancers;
    
    public void OnPrepareStepAction(int beat, BeatManager.BeatType type, DanceStep step)
    {
        
        foreach (Dancer dancer in dancers)
        {
            dancer.OnEnablePuzzle(null);
            dancer.OnPrepareStepAction(beat, type,step);
        }
    }
    public void OnDanceStepAction(int beat, BeatManager.BeatType type, DanceStep step)
    {
        foreach (Dancer dancer in dancers)
        {
            dancer.OnDanceStepAction(beat, type,step);
        }
    }
    public void OnReleaseStepAction(int beat, BeatManager.BeatType type, DanceStep step)
    {
        foreach (Dancer dancer in dancers)
        {
            dancer.OnReleaseStepAction(beat, type,step);
            dancer.OnDisablePuzzle(null);
        }
    }
}


public class FollowPuzzle : RhythmPuzzle
{
    [Header("Follow Puzzle Settings")]
    public Dancer leader;
    public bool leaderTurn;
    private int innerBeatCounter=0;
    
    public List<FollowDanceSequence> followDanceSequences = new List<FollowDanceSequence>();
    
    public override void PreparePuzzle()
    {
        currentSequenceIndex = 0;
        innerBeatCounter = 0;
        leaderTurn = true;
        eventManager.AddListener(leader);
        SetSequence(followDanceSequences[currentSequenceIndex].danceSequence);
    }

    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        leaderTurn = true;
        innerBeatCounter = 0;
        
    }

    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount == 1) availableToDance = true; 
        if (!availableToDance) return;
        currentStep = currentDanceSequence.GetDanceStep(innerBeatCounter,type);
        if(leaderTurn) eventManager.InvokePrepare(beat, type, currentStep);
        else followDanceSequences[currentSequenceIndex].OnPrepareStepAction(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        if(leaderTurn) eventManager.InvokeDance(beat, type, currentStep);
        else followDanceSequences[currentSequenceIndex].OnDanceStepAction(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        if(leaderTurn) eventManager.InvokeRealease(beat, type, currentStep);
        else followDanceSequences[currentSequenceIndex].OnReleaseStepAction(beat,type, currentStep);
        innerBeatCounter++;
        if (innerBeatCounter == currentDanceSequence.coreography.StepInBar.Count)SetNextDancer();
    }

    public void SetNextDancer()
    {
        if(!leaderTurn) AllDancersHasDanced();
        leaderTurn = !leaderTurn;
        innerBeatCounter = 0;
    }

    public void AllDancersHasDanced()
    {
        //Debug.Log("AllDancersHasDanced");
        bool hasEnded = followDanceSequences[currentSequenceIndex].danceSequence.CheckEndOfSequence(innerBeatCounter);
        
        if(hasEnded)
        {
            //Debug.Log("Sequence has been ended");
            currentSequenceIndex++;
            if(currentSequenceIndex < followDanceSequences.Count) SetSequence(followDanceSequences[currentSequenceIndex].danceSequence);
            else OnPuzzleCompleted();
            
        }
    }
}
