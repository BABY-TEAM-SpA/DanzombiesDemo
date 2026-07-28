using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FollowSequence
{
    public bool playerAffected = false;
    public List<ZombieDanceBrain> zombiesAffected = new List<ZombieDanceBrain>();
    public SequenceStep danceSequence;
}


public class FollowPuzzle : RhythmPuzzle
{
    [Header("Follow Puzzle Settings")]
    public FeedbackElement playerFeedbackElement;
    public ZombieDanceBrain leader;
    private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();
    public bool leaderTurn;
    public List<FollowSequence> followSequences = new List<FollowSequence>();
    private FollowSequence currentFollowSequence;
    private int currentFollowSequenceIndex = 0;
    
    
    public override void OnUpdateSongAction()
    {
        //throw new System.NotImplementedException();
    }

    public override void PreBeatAction(int counter)
    {
        throw new NotImplementedException();
    }

    public override void BeatAction(int counter)
    {
        throw new NotImplementedException();
    }

    public override void PostBeatAction(int counter)
    {
        throw new NotImplementedException();
    }

    public override void PreparePuzzle() { }

    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp)
    {
        if (exp == currentReaction) return;
        currentReaction = exp;
    }
    
    

    protected override void CheckPlayerPost()
    {
        if (!PlayerHasDanced && playersInside != null && currentDanceData.DanceStep != DanceStep.None && currentFollowSequence.playerAffected && !leaderTurn)
        {
            currentDanceData.Sequence.ApplyDance(false);
            playersInside.ReportEmptyDance();
        }
    }

    public override void OnDanceSequenceCleared()
    {
        Debug.Log("CHeck if sequence end");
        if (leaderTurn)
        {
            if(currentFollowSequence.playerAffected) playerFeedbackElement.Activate(true);
            leaderTurn = false;
            currentDanceData.listeners.RemoveListener(leader);
            ZombieConnect(currentFollowSequence.zombiesAffected);
            ActivateFollowSequence(currentFollowSequenceIndex);
        }
        else
        {
            playerFeedbackElement.Activate(false);
            DisconnectAll();
            leaderTurn = true;
            currentDanceData.listeners.AddListener(leader);
            ActivateFollowSequence(currentFollowSequenceIndex+1);
        }
    }

    private void ZombieConnect(List<ZombieDanceBrain> brains)
    {
        zombies = brains;
        foreach (ZombieDanceBrain zombie in zombies)
        {
            currentDanceData.listeners.AddListener(zombie);
        }
    }

    private void DisconnectAll()
    {
        currentDanceData.listeners.RemoveListener(leader);
        foreach (var zombie in zombies)
        {
            currentDanceData.listeners.RemoveListener(zombie);
        }
        zombies.Clear();
    }
    
    private void OnDisable()
    {
        foreach (var zombie in zombies) currentDanceData.listeners.RemoveListener(zombie);
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
