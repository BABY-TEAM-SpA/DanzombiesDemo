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

    public override void OnUpdateSongAction() {}
    

    public override void ActivatePuzzle(bool activate)
    {
        HUD.SetActiveCanvas(activate);
        InnerCounter = 0;
        playerSucceses = 0;
        base.ActivatePuzzle(activate);
        if (currentTutorialSequence < TutorialSequences.Count) currentDanceData.Sequence = TutorialSequences[currentTutorialSequence];
        if (currentDanceData.Sequence.playbackMode == SequenceStep.PlaybackMode.LoopUntilFlowIsFull) playersInside?.ActivateDanceHUD(activate);;
    }

    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp) {}

    public override void PlayerGetDamaged() {}

    protected override void PuzzlePreBeat() {}

    protected override void PuzzleBeat() {}

    protected override void PuzzlePostBeat() {}

    public override void OnSequenceEnd()
    {
        currentDanceData.Sequence.OnSequenceCompletedEvent?.Invoke();
        currentTutorialSequence += 1;
        InnerCounter = 0;
        ActivatePuzzle(false);
    }
    
}
