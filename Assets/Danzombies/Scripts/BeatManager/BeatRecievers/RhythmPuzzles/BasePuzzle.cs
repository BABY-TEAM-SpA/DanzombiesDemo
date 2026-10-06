using UnityEngine;

public class BasePuzzle : RhythmPuzzle
{
    public Dancer dancer;
    public DanceSequence danceSequence;
    public int innerCounter=0;
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount==1)
        {
            innerCounter = 0;
            availableToDance = true;
        }
        if (!availableToDance) return;
        currentStep = currentDanceSequence.GetDanceStep(innerCounter,type);
        if(debug) Debug.Log($"PreBeat {beat}:{type}.... dance:{currentStep}");
        eventManager.InvokePrepare(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        if(debug) Debug.Log($"Beat {beat}:{type}.... dance:{currentStep}");
        eventManager.InvokeDance(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        if (availableToDance)
        {
            if(type== BeatManager.BeatType.FullBeat) innerCounter++;
            eventManager.InvokeRealease(beat, type, currentStep);
            if(debug) Debug.Log($"PostBeat {beat}:{type}.... dance:{currentStep}");
            if(currentDanceSequence.CheckEndOfSequence(innerCounter)) OnPuzzleCompleted();
        }
    }

    public override void PreparePuzzle()
    {
        currentDanceSequence = danceSequence; 
        eventManager.AddListener(dancer);
    }
    
}
