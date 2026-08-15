using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FollowSequence
{
    public bool playerAffected = false;
    public List<ZombieDanceBrain> zombiesAffected = new List<ZombieDanceBrain>();
    public DanceSequence danceSequence;
}


public class FollowPuzzle : RhythmPuzzle
{
    [Header("Follow Puzzle Settings")]
    public FeedbackElement playerFeedbackElement;
    public Dancer leader;
    public List<Dancer> zombies = new List<Dancer>();
    public bool leaderTurn;
    public List<FollowSequence> followSequences = new List<FollowSequence>();
    private FollowSequence currentFollowSequence;
    private int currentFollowSequenceIndex = 0;


    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        throw new NotImplementedException();
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        throw new NotImplementedException();
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        throw new NotImplementedException();
    }

    public override void PreparePuzzle()
    {
        throw new NotImplementedException();
    }

    public override void OnPuzzleCompleted()
    {
        throw new NotImplementedException();
    }
}
