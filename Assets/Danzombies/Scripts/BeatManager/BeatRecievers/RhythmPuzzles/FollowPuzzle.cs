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
        }
    }
}


public class FollowPuzzle : RhythmPuzzle
{
    [Header("Follow Puzzle Settings")]
    public Dancer leader;
    public bool leaderTurn;
    
    
    private int innerBeatCounter=0;
    int currentSequenceIndex = 0;
    
    public List<FollowDanceSequence> followDanceSequences = new List<FollowDanceSequence>();

    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        PreparePuzzle();
    }

    public override void PreparePuzzle()
    {
        currentSequenceIndex = 0;
        leaderTurn = true;
        SetSequence(followDanceSequences[currentSequenceIndex].danceSequence);
    }
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        currentStep = danceSequence.GetDanceStep(innerBeatCounter,type);
        if(leaderTurn) leader.OnPrepareStepAction(beat, type, currentStep);
        else followDanceSequences[currentSequenceIndex].OnPrepareStepAction(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if(leaderTurn) leader.OnDanceStepAction(beat, type, currentStep);
        else followDanceSequences[currentSequenceIndex].OnDanceStepAction(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        if(leaderTurn) leader.OnReleaseStepAction(beat, type, currentStep);
        else followDanceSequences[currentSequenceIndex].OnReleaseStepAction(beat,type, currentStep);
        innerBeatCounter++;
        if (innerBeatCounter == danceSequence.coreography.StepInBar.Count)SetNextDancer();
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
    public override void OnPuzzleCompleted()
    {
        //Debug.Log("Puzzle Is Over");
        SetActivePuzzle(false);
    }
}
