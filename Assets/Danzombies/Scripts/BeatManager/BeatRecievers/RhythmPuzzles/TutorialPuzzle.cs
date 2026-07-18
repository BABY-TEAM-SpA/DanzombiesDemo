using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;


public class TutorialPuzzle : RhythmPuzzle
{
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;
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

    public override void OnUpdateSongAction() {}
    

    public override void ActivatePuzzle(bool activate)
    {
        HUD.SetActiveCanvas(activate);
        InnerCounter = 0;
        base.ActivatePuzzle(activate);
        if (currentTutorialSequence < TutorialSequences.Count) SetSequence(TutorialSequences[currentTutorialSequence]);
    }

    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp) {}

    public override void PlayerGetDamaged() {}
    

    public override void OnDanceSequenceCleared()
    {
        Debug.Log("OnDanceSequenceCleared");
        currentTutorialSequence += 1;
        InnerCounter = 0;
        ActivatePuzzle(false);
    }
    
}
