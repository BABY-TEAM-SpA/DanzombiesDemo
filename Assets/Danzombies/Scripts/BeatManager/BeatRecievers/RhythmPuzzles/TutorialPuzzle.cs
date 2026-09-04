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

    

    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        if (currentSequenceIndex < TutorialSequences.Count) SetSequence(TutorialSequences[currentSequenceIndex]);
    }
    
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        Debug.Log(BeatManager.Instance.localBeatCount);
        Debug.Log(beat);
        currentStep = currentDanceSequence.GetDanceStep(beat-1,type);
        eventManager.InvokePrepare(beat,type,currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        if (!availableToDance) return;
        //Debug.Log(BeatManager.Instance.localBeatCount);
        eventManager.InvokeDance(beat,type,currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        if (availableToDance)
        {
            //Debug.Log(BeatManager.Instance.localBeatCount);
            eventManager.InvokeRealease(beat, type, currentStep);
            CheckEnd(beat);
        }
        if (isActive && !availableToDance && BeatManager.Instance.localBeatCount == BeatManager.Instance.globalUpperBar) availableToDance = true; 
    }
    
    public override void PreparePuzzle()
    {
        eventManager.AddListener(zone);
        //eventManager.AddListener(HUD);
    }
    
    public void CheckEnd(int beat)
    {
        if(TutorialSequences[currentSequenceIndex].CheckEndOfSequence(beat))OnPuzzleCompleted();
    }

 

    public void ActivatePuzzleByIndex(int index)
    {
        currentSequenceIndex = index;
        SetActivePuzzle(true);
    }
    
    #endregion
}
