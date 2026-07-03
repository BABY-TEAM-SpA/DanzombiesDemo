using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FollowSequence
{
    public bool playerAffected = false;
    public List<int> zombiesAffected = new List<int>();
    public SequenceStep danceSequence;
}

[Serializable]
public class ZombieDummie
{
    public ZombieDanceBrain brain;
    public FeedbackElement feedbackElement;
}

public class FollowPuzzle : RhythmPuzzle
{
    [Header("Follow Puzzle Settings")]
    public FeedbackElement playerFeedbackElement;
    public ZombieDummie leader;
    public bool leaderTurn;
    public List<ZombieDummie> zombies = new List<ZombieDummie>();
    public List<FollowSequence> followSequences = new List<FollowSequence>();
    private FollowSequence currentFollowSequence;
    private int currentFollowSequenceIndex = 0;

    public override void ActivatePuzzle(bool activate)
    {
        base.ActivatePuzzle(activate);
        playersInside.ActivateDanceHUD(true);
        ActivateFollowSequence(0);
        leaderTurn = true;
        leader.brain.Connect(this);
        leader.feedbackElement.Activate(true);
    }
    
    public override void OnUpdateSongAction()
    {
        //throw new System.NotImplementedException();
    }
    
    public override void PreparePuzzle() { }

    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp)
    {
        if (exp == currentReaction) return;
        currentReaction = exp;
        foreach (ZombieDummie zombie in zombies)
        {
            zombie.brain.React(currentReaction);
        }
    }

    public override void PlayerGetDamaged()
    {
        
    }

    protected override void PuzzlePreBeat() { }

    protected override void PuzzleBeat() { }

    protected override void PuzzlePostBeat() { }

    protected override void CheckPlayerPost()
    {
        if (!PlayerHasDanced && playersInside != null && currentDanceData.DanceStep != DanceStep.None && currentFollowSequence.playerAffected && !leaderTurn)
        {
            currentDanceData.Sequence.ApplyDance(false);
            playersInside.ReportEmptyDance();
        }
    }

    public override void OnSequenceEnd()
    {
        Debug.Log("CHeck if sequence end");
        if (leaderTurn)
        {
            if(currentFollowSequence.playerAffected) playerFeedbackElement.Activate(true);
            leaderTurn = false;
            leader.brain.Disconnect(this);
            leader.feedbackElement.Activate(false);
            ZombieConnect(currentFollowSequence.zombiesAffected);
            ActivateFollowSequence(currentFollowSequenceIndex);
        }
        else
        {
            playerFeedbackElement.Activate(false);
            DisconnectAll();
            leaderTurn = true;
            leader.brain.Connect(this);
            leader.feedbackElement.Activate(true);
            ActivateFollowSequence(currentFollowSequenceIndex+1);
        }
    }

    private void ZombieConnect(List<int> indexes)
    {
        foreach (int index in indexes)
        {
            var zombie = zombies[index]; 
            zombie.brain.Connect(this);
            zombie.feedbackElement.Activate(true);
        }
    }

    private void DisconnectAll()
    {
        leader.brain.Disconnect(this);
        leader.feedbackElement.Activate(false);
        foreach (var zombie in zombies)
        {
            zombie.brain.Disconnect(this);
            zombie.feedbackElement.Activate(false);
        }
    }
    
    private void OnDisable()
    {
        foreach (var zombie in zombies) zombie.brain.Disconnect(this);
    }
    
    private void ActivateFollowSequence(int index)
    {
        if (index >= followSequences.Count)
        {
            DisconnectAll();
            Debug.Log("Puzzle Has been completed");
            ActivatePuzzle(false);
            return;
        }
        currentFollowSequenceIndex = index;
        currentFollowSequence = followSequences[currentFollowSequenceIndex];
        SetSequence(currentFollowSequence.danceSequence);
    }
    
    public override bool SetPlayerInput(DanceStep playerStep, out BeatFeedback bf)
    { 
        bf = playerStep == currentDanceData.DanceStep? BeatManager.Instance.EvaluateInput(beatType): BeatFeedback.Bad;
        if (!PlayerHasDanced && isOnBeat && !leaderTurn && currentFollowSequence.playerAffected)
        {
            PlayerHasDanced = true;
            return true;
        }
        return false;
    }
    
    
    


}
