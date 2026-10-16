using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CanonPuzzle : RhythmPuzzle
{
    [SerializeField] private List<Dancer> dancers =new List<Dancer>();
    private int currentDancerIndex = 0;
    public List<DanceSequence> danceSequences = new List<DanceSequence>();
    public UnityEvent OnAvailable; 
    
    
    public override void PreparePuzzle()
    {
        currentSequenceIndex = 0;
        currentDancerIndex = 0;
    }
    
    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        currentDancerIndex = 0;
        innerBeatCounter = 0;
        SetSequence(danceSequences[currentSequenceIndex]);
        if(activate) dancers[0].OnEnablePuzzle(this);
    }
    
    public override void BeatAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs)
    {
        if (!availableToDance) return;
        GetDanceStep(beatCounter,beatType);
        if(debug) Debug.Log($"Beat {beatCounter} (inner:{innerBeatCounter}):{beatType}.... dance:{currentStep}");
        dancers[currentDancerIndex].OnDanceStepAction(beatCounter,beatType, beatTimeMs,currentStep);
    }

    public override void PostBarAction(int beatCounter)
    {
        if (availableToDance)
        {
            innerBeatCounter++;
            if (beatCounter >= currentDanceSequence.coreography.StepInBar.Count) SetNextDancer();
        }
        if (isActive && !availableToDance && beatCounter == BeatManager.Instance.globalUpperBar)
        {
            availableToDance = true;
            innerBeatCounter = 0;
            OnAvailable?.Invoke();
            currentDanceSequence.ActivateSequence();
        };
        
    }

    public void SetNextDancer()
    {
        dancers[currentDancerIndex].OnDisablePuzzle(this);
        if(currentDancerIndex+1 >= dancers.Count)
        {
            //Debug.Log("AllDancersHasDanced");
            if (danceSequences[currentSequenceIndex].CheckEndOfSequence(innerBeatCounter))
            {
                //Debug.Log("activateByIndex");
                if (currentSequenceIndex+1 >= danceSequences.Count)
                {
                    //Debug.Log("EndOof Puzzle");
                    OnPuzzleCompleted();
                    return;
                }
                else
                {
                    //Debug.Log("NextSequence");
                    ActivatePuzzleByIndex(currentSequenceIndex+1);
                }
            }
            else
            {
                //Debug.Log("Loop");
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
