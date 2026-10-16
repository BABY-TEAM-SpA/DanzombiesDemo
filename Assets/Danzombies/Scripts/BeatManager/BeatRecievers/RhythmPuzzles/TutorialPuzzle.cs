using System.Collections.Generic;
using UnityEngine;

public class TutorialPuzzle : RhythmPuzzle
{
    #region [VARIABLES]
    [SerializeField] private DanceZone zone;
    
    [Header("Tutorial Dance Settings")]
    public List<DanceSequence> TutorialSequences = new List<DanceSequence>();
    
    #endregion
    #region [METHODS]

    public override void PreparePuzzle()
    {
        danceEventBuffer.AddListener(zone);
    }

    public void ActivatePuzzleByIndex(int index)
    {
        currentSequenceIndex = index;
        SetActivePuzzle(true);
    }
    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        if (currentSequenceIndex < TutorialSequences.Count) SetSequence(TutorialSequences[currentSequenceIndex]);
    }

    public override void BeatAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs)
    {
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount==1)
        {
            innerBeatCounter = 0;
            availableToDance = true;
        }
        if (!availableToDance) return;
        if(beatType== BeatManager.BeatType.FullBeat) innerBeatCounter++;
        GetDanceStep(beatCounter,beatType);
        if(debug) Debug.Log($"Beat {beatCounter} (inner:{innerBeatCounter}):{beatType}.... dance:{currentStep}");
        danceEventBuffer.InvokeDance(beatCounter,beatType,beatTimeMs,currentStep);
        GetNextDanceStep(beatCounter,beatType);
        
    }
    protected override void CheckEnd(int beat)
    {
        //if(TutorialSequences[currentSequenceIndex].CheckEndOfSequence(beat))OnPuzzleCompleted();
        if(currentDanceSequence.CheckEndOfSequence(beat))OnPuzzleCompleted();
    }
    
    #endregion
}
