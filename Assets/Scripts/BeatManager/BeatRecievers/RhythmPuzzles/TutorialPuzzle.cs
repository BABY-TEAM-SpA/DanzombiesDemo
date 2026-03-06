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

    public override void ActivatePuzzle(bool activate)
    {
        innerCounter = 0;
        playerSucceses = 0;
        base.ActivatePuzzle(activate);
        if (currentTutorialSequence < TutorialSequences.Count)
            activeDanceSequence = TutorialSequences[currentTutorialSequence];
    }

    public override void VisualFeedbackToPlayerDance(bool isCorrect)
    {
        //throw new NotImplementedException();
    }

    public override void GeneralVisualFeedback(int counter)
    {
        //throw new NotImplementedException();
    }
    
    public override void PlayerHasNoFlow(PlayerManager player)
    {
        //throw new NotImplementedException();
    }
    
    public override void ReactToPlayersDance(PlayerManager player, DanceStep step)
    {
        if (step == DanceStep.None)return;
        bool IsPlayerDanceCorrect = player.saveDanceStep == step;
        Debug.Log("Puzzle: "+step.ToString()+ "| Player: "+ player.saveDanceStep.ToString()+ " | IsPlayerDanceCorrect: " + IsPlayerDanceCorrect);
        VisualFeedbackToPlayerDance(IsPlayerDanceCorrect);
        MissionBuffer(IsPlayerDanceCorrect);
    }

    public void MissionBuffer(bool isCorrect)
    {
        switch (activeDanceSequence.goalType)
        {
            default:
                if (isCorrect) CompleteRhythmSequence();
                break;
            case SequenceStep.GoalType.CorrectXTimes:
                if (isCorrect)
                {
                    playerSucceses+=1;
                    if (playerSucceses >= puzzleGoal)
                    {
                        playerSucceses = 0;
                        CompleteRhythmSequence();
                    }
                }
                else
                {
                    playerSucceses = 0;
                }

                break;
            case SequenceStep.GoalType.CompleteFullPattern:
                if(isCorrect) playerSucceses+=1;
                int totaldances = activeDanceSequence.DanceSteps.FindAll(x=>x!=DanceStep.None).Count;
                if (innerCounter==activeDanceSequence.DanceSteps.Count-1)
                {
                    if(playerSucceses == totaldances) CompleteRhythmSequence();
                    playerSucceses=0;
                }

                break;
            case SequenceStep.GoalType.FillFlow:
                int flow = playersInside[0].GetFlowDamage(!isCorrect?1:-1);
                if(flow == 10) CompleteRhythmSequence();
                break;
        }
    }
    

    public void CompleteRhythmSequence()
    {
        activeDanceSequence.OnSequenceCompletedEvent?.Invoke();
        currentTutorialSequence += 1;
        innerCounter = 0;
        ActivatePuzzle(false);
        
    }
}
