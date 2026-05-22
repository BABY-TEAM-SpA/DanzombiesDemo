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

public class FollowPuzzle : RhythmPuzzle
{
    [Header("Follow Puzzle Settings")]
    public ZombieDanceBrain leader;
    public bool leaderTurn;
    public List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();
    public List<FollowSequence> followSequences = new List<FollowSequence>();
    private FollowSequence currentFollowSequence;
    private int currentFollowSequenceIndex = 0;

    public override void ActivatePuzzle(bool activate)
    {
        base.ActivatePuzzle(activate);
        ActivateFollowSequence(0);
        leaderTurn = true;
        leader.Connect(this);
    }
    
    public override void OnUpdateSongAction()
    {
        //throw new System.NotImplementedException();
    }
    
    public override void PreparePuzzle() { }

    protected override void PuzzlePreBeat() { }

    protected override void PuzzleBeat() { }

    protected override void PuzzlePostBeat() { }

    public override void OnSequenceEnd()
    {
        Debug.Log("CHeck if sequence end");
        if (leaderTurn)
        {
            leaderTurn = false;
            leader.Disconnect(this);
            ZombieConnect(currentFollowSequence.zombiesAffected);
            ActivateFollowSequence(currentFollowSequenceIndex);
        }
        else
        {
            DisconnectAll();
            leaderTurn = true;
            leader.Connect(this);
            ActivateFollowSequence(currentFollowSequenceIndex+1);
        }
    }

    private void ZombieConnect(List<int> indexes)
    {
        foreach (int index in indexes) zombies[index].Connect(this);
    }

    private void DisconnectAll()
    {
        leader.Disconnect(this);
        foreach (var zombie in zombies) zombie.Disconnect(this);
    }
    
    private void OnDisable()
    {
        foreach (ZombieDanceBrain zombie in zombies) zombie.Disconnect(this);
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
        bf = playerStep == CurrentPuzzleStep? BeatManager.Instance.EvaluateInput(beatType): BeatFeedback.Bad;
        if (!PlayerHasDanced && isOnBeat && !leaderTurn && currentFollowSequence.playerAffected)
        {
            PlayerHasDanced = true;
            return true;
        }
        return false;
    }
    
    
    


}
