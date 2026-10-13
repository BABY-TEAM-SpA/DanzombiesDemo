using UnityEngine;

public class BasePuzzle : RhythmPuzzle
{
    public Dancer dancer;
    public DanceSequence danceSequence;
    public int innerCounter=0;
    
   
    public override void BeatAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs)
    {
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount==1)
        {
            innerCounter = 0;
            availableToDance = true;
        }
        if (!availableToDance) return;
        currentStep = currentDanceSequence.GetDanceStep(innerCounter,beatType);
        if(debug) Debug.Log($"Beat {beatCounter}:{beatType}.... dance:{currentStep}");
        danceEventBuffer.InvokeDance(beatCounter,beatType,beatTimeMs,currentStep);
        if(beatType== BeatManager.BeatType.FullBeat) innerCounter++;
        //if(currentDanceSequence.CheckEndOfSequence(innerCounter)) OnPuzzleCompleted();
    }

    public override void PreparePuzzle()
    {
        currentDanceSequence = danceSequence; 
        danceEventBuffer.AddListener(dancer);
    }
    
}
