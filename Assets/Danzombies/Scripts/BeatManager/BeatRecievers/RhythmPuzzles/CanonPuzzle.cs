using System.Collections.Generic;
using UnityEngine;


public class CanonPuzzle : RhythmPuzzle
{
    [SerializeField] private List<Dancer> dancers =new List<Dancer>();
    private int currentDancerIndex = 0;
    
    private int innerBeatCounter=0;
    int currentSequenceIndex = 0;
    
    
    public List<DanceSequence> danceSequences = new List<DanceSequence>();

    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        PreparePuzzle();
    }

    public override void PreparePuzzle()
    {
        currentSequenceIndex = 0;
        currentDancerIndex = 0;
        SetSequence(danceSequences[currentSequenceIndex]);
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
            SetNextDancer();
        }
    }

    public void SetNextDancer()
    {
        if(currentDancerIndex+1== dancers.Count) AllDancersHasDanced();
        currentDancerIndex = (currentDancerIndex+1)%dancers.Count;
        innerBeatCounter = 0;
    }

    public void AllDancersHasDanced()
    {
        //Debug.Log("AllDancersHasDanced");
        bool hasEnded = danceSequences[currentSequenceIndex].CheckEndOfSequence(innerBeatCounter);
        
        if(hasEnded)
        {
            //Debug.Log("Sequence has been ended");
            currentSequenceIndex++;
            if(currentSequenceIndex < danceSequences.Count) SetSequence(danceSequences[currentSequenceIndex]);
            else OnPuzzleCompleted();
            
        }
    }
    public override void OnPuzzleCompleted()
    {
        //Debug.Log("Puzzle Is Over");
        SetActivePuzzle(false);
    }
    
    
}
