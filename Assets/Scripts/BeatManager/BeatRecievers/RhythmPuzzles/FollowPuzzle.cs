using System;
using System.Collections.Generic;
using UnityEngine;

public enum Subs
{
    Player,
    SubA,
    SubB,
    SubC,
}


[Serializable]
public class FollowSequence
{
    public List<Subs> affected = new List<Subs>();
    public SequenceStep danceSequence;
}

public class FollowPuzzle : RhythmPuzzle
{
    
    
    public ZombieDanceBrain leader;
    public List<FollowSequence> danceSequence = new List<FollowSequence>();
    
    public ZombieDanceBrain SubA;
    public ZombieDanceBrain SubB;
    public ZombieDanceBrain SubC;
    
    private bool leaderTurn;
    
    
    
    public override void OnUpdateSongAction()
    {
        throw new System.NotImplementedException();
    }

    public override void OnPauseSongAction()
    {
        throw new System.NotImplementedException();
    }

    public override void OnResumeAction()
    {
        throw new System.NotImplementedException();
    }

    public override void OnStopSongAction()
    {
        throw new System.NotImplementedException();
    }

    public override void PreparePuzzle()
    {
        leader.Connect(this);
        SubA.Connect(this);
        SubB.Connect(this);
        SubC.Connect(this);
    }

    public override void GeneralVisualFeedback(int counter)
    {
        throw new System.NotImplementedException();
    }

}
