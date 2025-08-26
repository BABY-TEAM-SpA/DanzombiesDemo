using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TutorialPuzzle : RhythmPuzzle
{
    public List<SequenceStep> TutorialSteps = new List<SequenceStep>();
    public int currentTutorialStepIndex = 0;

    [SerializeField] private int playerSucceses=0;
    [SerializeField] private Color SuccesColor = Color.yellow;
    [SerializeField] private Color FailureColor = Color.red;
    
    public UnityAction OnSequenceCompletedEvent;
    
    [SerializeField] private ZombieDanceBrain Steph;
    
    
    private void Start()
    {
        Steph?.Connect(this);
        player.OnDance += OnPlayerInputAction;
    }

    private void OnDisable()
    {
        Steph?.Disconnect(this);
        player.OnDance -= OnPlayerInputAction;
    }
    public override void ActivatePuzzle(bool activate)
    {
       
        base.ActivatePuzzle(activate);
        if(currentTutorialStepIndex< TutorialSteps.Count) DanceSteps = TutorialSteps[currentTutorialStepIndex].DanceSteps;
    }
    
    
    public override void PlayerDanceReaction(bool IsPlayerDanceCorrect)
    {
        if (feedBack != null)
        {
            feedBack.color=(IsPlayerDanceCorrect)?SuccesColor:FailureColor;   
        }
        
        if (IsPlayerDanceCorrect)
        {
            playerSucceses+=1;
            if (playerSucceses >= 2)
            {
                playerSucceses = 0;
                OnRhythmSequenceCompleted();
            }
        }
        else
        {
            playerSucceses = 0;
        }
    }

    private void OnRhythmSequenceCompleted()
    {
        currentTutorialStepIndex += 1;
        
        if (currentTutorialStepIndex >= TutorialSteps.Count)
        {
            ActivatePuzzle(false);
            OnRhythmPuzzleCompletedEvent?.Invoke();
        }
        else
        {
            //ActivatePuzzle(false);
            DanceSteps = TutorialSteps[currentTutorialStepIndex].DanceSteps;
            OnSequenceCompletedEvent?.Invoke();
        }

    }
}
