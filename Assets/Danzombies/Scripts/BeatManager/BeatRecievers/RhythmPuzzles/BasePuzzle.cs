using UnityEngine;

public class BasePuzzle : RhythmPuzzle
{
    public Dancer dancer;
    public DanceSequence danceSequence;
    
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount == 1) availableToDance = true; 
        if (!availableToDance) return;
        currentStep = currentDanceSequence.GetDanceStep(beat,type);
        eventManager.InvokePrepare(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        eventManager.InvokeDance(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        eventManager.InvokeRealease(beat,type,currentStep);
    }

    public override void PreparePuzzle()
    {
        currentDanceSequence = danceSequence; 
        eventManager.AddListener(dancer);
    }
    
}
