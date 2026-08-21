using UnityEngine;

public class BasePuzzle : RhythmPuzzle
{
    public Dancer dancer;
    
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        currentStep = danceSequence.GetDanceStep(beat,type);
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
        eventManager.AddListener(dancer);
    }
    
}
