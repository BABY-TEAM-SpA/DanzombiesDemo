using System.Collections.Generic;
using UnityEngine;


public class CanonPuzzle : RhythmPuzzle
{
    [SerializeField] private List<Dancer> dancers =new List<Dancer>();
    private int currentDancerIndex = 0;
    
    private int innerBeatCounter=0;
    int currentSequenceIndex = 0;
    private bool availableToDance = false;
    public List<DanceSequence> danceSequences = new List<DanceSequence>();

    
    
    public override void PreparePuzzle()
    {
        currentSequenceIndex = 0;
        currentDancerIndex = 0;
    }
    public void ActivatePuzzleByIndex(int index)
    {
        Debug.Log("ActivatePuzzleByIndex"+index);
        currentSequenceIndex = index;
        SetActivePuzzle(true);
    }
    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        availableToDance = false;
        currentDancerIndex = 0;
        innerBeatCounter = 0;
        SetSequence(danceSequences[currentSequenceIndex]);
        if(activate) dancers[0].OnEnablePuzzle(this);
    }
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount == 1) availableToDance = true;
        if (!availableToDance) return;
        currentStep = danceSequence.GetDanceStep(innerBeatCounter,type);
        dancers[currentDancerIndex].OnPrepareStepAction(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        dancers[currentDancerIndex].OnDanceStepAction(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        dancers[currentDancerIndex].OnReleaseStepAction(beat,type, currentStep);
        innerBeatCounter++;
        if (innerBeatCounter >= danceSequence.coreography.StepInBar.Count) SetNextDancer();
    }

    public void SetNextDancer()
    {
        dancers[currentDancerIndex].OnDisablePuzzle(this);
        if(currentDancerIndex+1 >= dancers.Count)
        {
            Debug.Log("AllDancersHasDanced");
            if (danceSequences[currentSequenceIndex].CheckEndOfSequence(innerBeatCounter))
            {
                Debug.Log("activateByIndex");
                if (currentSequenceIndex+1 >= danceSequences.Count)
                {
                    Debug.Log("EndOof Puzzle");
                    OnPuzzleCompleted();
                    return;
                }
                else
                {
                    Debug.Log("NextSequence");
                    ActivatePuzzleByIndex(currentSequenceIndex+1);
                }
            }
            else
            {
                Debug.Log("Loop");
                ActivatePuzzleByIndex(currentSequenceIndex);
            }
        }
        else
        {
            currentDancerIndex +=1;
            innerBeatCounter = 0;
            dancers[currentDancerIndex].OnEnablePuzzle(this);
        }
    }
}
