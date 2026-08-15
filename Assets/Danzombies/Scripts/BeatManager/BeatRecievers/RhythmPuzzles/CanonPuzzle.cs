using System.Collections.Generic;
using UnityEngine;


public class CanonPuzzle : RhythmPuzzle
{
    [SerializeField] private List<Dancer> dancers =new List<Dancer>();
    private int currentDancerIndex = 0;
    
    int currentSequenceIndex = 0;
    private int innerCounter=0;
    
    public List<DanceSequence> danceSequences = new List<DanceSequence>();
    
    public override void PreparePuzzle()
    {

        currentSequenceIndex = 0;
        currentDancerIndex = 0;
        SetSequence(danceSequences[currentSequenceIndex]);
    }

    public override void OnPuzzleCompleted()
    {
        throw new System.NotImplementedException();
    }


    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        currentStep = danceSequence.GetDanceStep(innerCounter,type);
        dancers[currentDancerIndex].OnPrepareStepAction(currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        dancers[currentDancerIndex].OnDanceStepAction(currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        dancers[currentDancerIndex].OnReleaseStepAction(currentStep);
        innerCounter++;
        if (innerCounter == danceSequence.coreography.StepInBar.Count)
        {
            innerCounter = 0;
            SetNextDancer();
        }
    }

    public void SetNextDancer()
    {
        if(currentDancerIndex+1== dancers.Count) AllDancersHasDanced();
        currentDancerIndex = (currentDancerIndex+1)%dancers.Count;
    }

    public void AllDancersHasDanced()
    {
        currentSequenceIndex++;
    }
    
    
}
