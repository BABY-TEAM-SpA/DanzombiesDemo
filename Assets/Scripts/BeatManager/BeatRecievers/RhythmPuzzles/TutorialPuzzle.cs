using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;


public class TutorialPuzzle : RhythmPuzzle
{
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;
    [SerializeField] private int playerSucceses = 0;
    public int puzzleGoal;
    [HideInInspector] public int currentTutorialSequence = 0;
    
    [Header("Tutorial Dance Settings")]
    public List<SequenceStep> TutorialSequences = new List<SequenceStep>();


    public override void PreparePuzzle()
    {
        Steph?.Connect(this);
        HUD?.Connect(this);
    }

    private void OnDisable()
    {
        Steph?.Disconnect(this);
        HUD?.Disconnect(this);
    }

    public override void OnUpdateSongAction()
    {
        //throw new NotImplementedException();
    }
    

    public override void ActivatePuzzle(bool activate)
    {
        InnerCounter = 0;
        playerSucceses = 0;
        base.ActivatePuzzle(activate);
        if (currentTutorialSequence < TutorialSequences.Count) CurrentSequence = TutorialSequences[currentTutorialSequence];
    }

    protected override void PuzzlePreBeat()
    {
        //throw new NotImplementedException();
    }

    protected override void PuzzleBeat()
    {
        //throw new NotImplementedException();
    }

    protected override void PuzzlePostBeat()
    {
        //throw new NotImplementedException();
    }

    public override void OnSequenceEnd()
    {
        //throw new NotImplementedException();
    }


    /*public void ReactToPlayersDance(PlayerManager player, DanceStep step)
    {
        if (step == DanceStep.None)return;
        bool IsPlayerDanceCorrect = player.saveDanceStep == step;
        Debug.Log("Puzzle: "+step.ToString()+ "| Player: "+ player.saveDanceStep.ToString()+ " | IsPlayerDanceCorrect: " + IsPlayerDanceCorrect);
        MissionBuffer(IsPlayerDanceCorrect);
    }*/
    

    public void CompleteRhythmSequence()
    {
        CurrentSequence.OnSequenceCompletedEvent?.Invoke();
        currentTutorialSequence += 1;
        InnerCounter = 0;
        ActivatePuzzle(false);
        
    }
}
