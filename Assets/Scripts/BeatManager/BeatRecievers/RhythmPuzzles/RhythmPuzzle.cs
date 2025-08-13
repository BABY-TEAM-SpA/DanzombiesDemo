using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum DanceStep
{
    None,
    L_North,
    R_North,
    L_South,
    R_South,
    L_West,
    R_West,
    L_East,
    R_East
}

public class RhythmPuzzle : BeatReciever
{
    private bool isActive = false;
    [SerializeField] bool ActivateOnStart;
    [SerializeField] protected bool useCompass = false;
    [SerializeField] protected bool ShouldRepeat =false;
    [SerializeField] protected List<DanceStep> DanceSteps = new List<DanceStep>();
    protected DanceStep currentDanceStep = DanceStep.None;
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public event OnMusicEvent OnReleaseStep;
    public UnityEvent OnRhythmPuzzleCompleted;
    [SerializeField] protected PlayerAnimatorController player;
    [SerializeField] protected DanceStep playerDanceStep;

    private void Awake()
    {
        if(ActivateOnStart) isActive = true;
    }

    public override void PreBeatAction(int counter, int counterCompass)
    {
        if (isActive)
        {
            if (useCompass) currentDanceStep = DanceSteps[counterCompass];
            else
            {
                int aux = counter-1;
                if (ShouldRepeat) aux = counter % DanceSteps.Count;
                currentDanceStep = DanceSteps[aux];
            }
            OnPrepareStep?.Invoke(currentDanceStep);
            
        }
    }

    public override void BeatAction(int counter, int counterCompass)
    {
        if (isActive)
        {
            OnDanceStep?.Invoke(currentDanceStep);
            if(OnDanceStep != null) Debug.Log("DanceZombies");
        }
        
    }

    public override void PostBeatAction(int counter, int counterCompass)
    {
        playerDanceStep = DanceStep.None;
        if (isActive)
        {
            OnReleaseStep?.Invoke(currentDanceStep);
        }
    }

    public virtual void OnPlayerInputAction(DanceStep step)
    {
        if (isActive)
        {
            playerDanceStep = step;
        }
    }
    
}
