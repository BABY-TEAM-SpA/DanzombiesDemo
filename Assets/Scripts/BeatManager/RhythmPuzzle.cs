using System.Collections.Generic;
using UnityEngine;


public enum DanceStep
{
    None,
    UpL,
    UpR,
    DownL,
    DownR,
    LeftL,
    LeftR,
    RightL,
    RightR
}

public class RhythmPuzzle : BeatReciever
{
    [SerializeField] private bool ShouldRepeat =false;
    
    [SerializeField] private List<DanceStep> DanceSteps = new List<DanceStep>();
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnDanceStep; 

    public override void BeatAction(int counter, int counterCompass)
    {
        //if()
        OnDanceStep?.Invoke(DanceSteps[counter]);
    }
}
