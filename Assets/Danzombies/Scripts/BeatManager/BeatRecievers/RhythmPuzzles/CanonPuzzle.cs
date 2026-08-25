using System.Collections.Generic;
using UnityEngine;


public class CanonPuzzle : RhythmPuzzle
{
    [SerializeField] private List<Dancer> dancers =new List<Dancer>();
    private int currentDancerIndex = 0;
    
    private int innerBeatCounter=0;
    int currentSequenceIndex = 0;
    public List<DanceSequence> danceSequences = new List<DanceSequence>();
    
    
    public override void PreparePuzzle()
    {
        currentSequenceIndex = 0;
        currentDancerIndex = 0;
        SetSequence(danceSequences[currentSequenceIndex]);
    }
    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        //PreparePuzzle();
        dancers[currentDancerIndex].OnEnablePuzzle(this);
    }

    
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        currentStep = danceSequence.GetDanceStep(innerBeatCounter,type);
        dancers[currentDancerIndex].OnPrepareStepAction(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        dancers[currentDancerIndex].OnDanceStepAction(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        dancers[currentDancerIndex].OnReleaseStepAction(beat,type, currentStep);
        innerBeatCounter++;
        if (innerBeatCounter == danceSequence.coreography.StepInBar.Count)
        {
            dancers[currentDancerIndex].OnDisablePuzzle(this);
            SetNextDancer();
        }
    }

    public void SetNextDancer()
    {
        bool hasEnded = false;
        if(currentDancerIndex+1!= dancers.Count)
        {
            currentDancerIndex = (currentDancerIndex+1)%dancers.Count;
            innerBeatCounter = 0;
            dancers[currentDancerIndex].OnEnablePuzzle(this);
            return;

        }
        AllDancersHasDanced();
        
    }

    public void AllDancersHasDanced()
    {
        if(danceSequences[currentSequenceIndex].CheckEndOfSequence(innerBeatCounter))
        {
            currentDancerIndex=0;
            currentSequenceIndex++;
            if(currentSequenceIndex < danceSequences.Count) SetSequence(danceSequences[currentSequenceIndex]);
            else OnPuzzleCompleted();
            
        }
    }
    
    
    
}
