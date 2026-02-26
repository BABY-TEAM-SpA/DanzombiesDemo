using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TutorialPuzzle : RhythmPuzzle
{
    public List<SequenceStep> TutorialSequences = new List<SequenceStep>();
    public int currentTutorialStepIndex = 0;
    [SerializeField] private int playerSucceses = 0;
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;


    private void Start()
    {
        Steph?.Connect(this);
        HUD?.Connect(this);

        //player.OnDance += OnPlayerInputAction;
    }

    private void OnDisable()
    {
        Steph?.Disconnect(this);
        //player.OnDance -= OnPlayerInputAction;
    }

    public override void ActivatePuzzle(bool activate)
    {
        playerSucceses = 0;
        base.ActivatePuzzle(activate);
        if (currentTutorialStepIndex < TutorialSequences.Count)
            currentDanceSequence = TutorialSequences[currentTutorialStepIndex];
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
        currentDanceSequence.OnSequenceCompletedEvent?.Invoke();
        currentTutorialStepIndex += 1;
        ActivatePuzzle(false);
        
    }
}
