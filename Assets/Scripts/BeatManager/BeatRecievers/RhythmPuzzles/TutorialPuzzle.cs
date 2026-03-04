using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TutorialPuzzle : RhythmPuzzle
{
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;
    [SerializeField] private int playerSucceses = 0;
    public int currentTutorialSequence = 0;
    
    [Header("Tutorial Dance Settings")]
    public List<SequenceStep> TutorialSequences = new List<SequenceStep>();
    
    private void Start()
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
        if (step == DanceStep.None)
        {
            return;
        }
        bool IsPlayerDanceCorrect = player.saveDanceStep == step;
        VisualFeedbackToPlayerDance(IsPlayerDanceCorrect);
        if (IsPlayerDanceCorrect)
        {
            playerSucceses+=1;
            if (playerSucceses >= 4)
            {
                playerSucceses = 0;
                CompleteRhythmSequence();
            }
        }
        else
        {
            playerSucceses = 0;
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
