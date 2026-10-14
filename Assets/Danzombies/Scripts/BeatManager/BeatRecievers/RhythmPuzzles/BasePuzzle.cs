using UnityEngine;

public class BasePuzzle : RhythmPuzzle
{
    public Dancer dancer; //zone
    public DanceSequence danceSequence;
    
   
    public override void BeatAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs) /// { FullBeat=0, FirstEmpty=1, FirstThird=2, HalfBeat=3, SecondThird=4, SecondEmpty=5 }
    {
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount==1)
        {
            innerBeatCounter = 0;
            availableToDance = true;
        }
        if (!availableToDance) return;
        currentStep = currentDanceSequence.GetDanceStep(innerBeatCounter,beatType);
        if(debug) Debug.Log($"Beat {beatCounter}:{beatType}.... dance:{currentStep}");
        danceEventBuffer.InvokeDance(beatCounter,beatType,beatTimeMs,currentStep);
        if(beatType== BeatManager.BeatType.FullBeat) innerBeatCounter++;
        //if(currentDanceSequence.CheckEndOfSequence(innerCounter)) OnPuzzleCompleted();
    }

    public override void PreparePuzzle()
    {
        currentDanceSequence = danceSequence; 
        danceEventBuffer.AddListener(dancer);
    }
    
}
