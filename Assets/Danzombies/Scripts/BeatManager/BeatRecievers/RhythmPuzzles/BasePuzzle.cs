using UnityEngine;

public class BasePuzzle : RhythmPuzzle
{
    public Dancer dancer;
    public DanceSequence danceSequence;
    public int innerCounter=0;
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        currentStep = currentDanceSequence.GetDanceStep(innerCounter,type);
        eventManager.InvokePrepare(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        eventManager.InvokeDance(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        if (availableToDance)
        {
            innerCounter++;
            eventManager.InvokeRealease(beat, type, currentStep);
        }
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount == 4) availableToDance = true; 
    }

    public override void PreparePuzzle()
    {
        currentDanceSequence = danceSequence; 
        eventManager.AddListener(dancer);
    }
    
}
