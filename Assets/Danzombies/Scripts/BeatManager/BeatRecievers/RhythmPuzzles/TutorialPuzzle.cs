using System.Collections.Generic;
using UnityEngine;

public class TutorialPuzzle : RhythmPuzzle
{
    #region [VARIABLES]
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;
    public int puzzleGoal;
    [HideInInspector] public int currentTutorialSequence = 0;
    
    [Header("Tutorial Dance Settings")]
    public List<DanceSequence> TutorialSequences = new List<DanceSequence>();
    
    PlayerManager player;
    
    #endregion


    private void OnDisable()
    {
        eventManager.RemoveAllListeners();
    }

    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        throw new System.NotImplementedException();
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        throw new System.NotImplementedException();
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        throw new System.NotImplementedException();
    }


    #region [METHODS]
    public override void PreparePuzzle()
    {
        eventManager.AddListener(Steph);
        eventManager.AddListener(HUD);
    }

    public override void SetActivePuzzle(bool activate)
    {
        HUD.SetActiveCanvas(activate);
        base.SetActivePuzzle(activate);
        if (currentTutorialSequence < TutorialSequences.Count) SetSequence(TutorialSequences[currentTutorialSequence]);
    }

    public override void OnPuzzleCompleted()
    {
        throw new System.NotImplementedException();
    }

    public void ActivatePuzzleByIndex(int index)
    {
        currentTutorialSequence = index;
        SetActivePuzzle(true);
    }
    
    #endregion
}
