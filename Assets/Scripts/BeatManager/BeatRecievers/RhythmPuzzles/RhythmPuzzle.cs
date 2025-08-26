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

[Serializable]
public class SequenceStep
{
    
    [SerializeField] public List<DanceStep> DanceSteps = new List<DanceStep>();
}

public class RhythmPuzzle : BeatReciever
{
    protected bool isActive = false;
    protected bool canRecieveInput = false;
    protected bool hasRecieveInput = false;
    [SerializeField] bool ActivateOnStart;
    [SerializeField] protected bool useCompass = false;
    [SerializeField] protected bool ShouldRepeat =false;
    [SerializeField] protected List<DanceStep> DanceSteps = new List<DanceStep>();
    protected DanceStep currentPuzzleStep = DanceStep.None;
    
    [Header("Rhythm Puzzle Events")]
    public UnityEvent OnRhythmPuzzleStartedEvent = new UnityEvent();
    public UnityEvent OnRhythmPuzzleCompletedEvent = new UnityEvent();
    public UnityEvent OnRhythmPuzzleFailedEvent = new UnityEvent();
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public event OnMusicEvent OnReleaseStep;
    
    [Header("Player References")]
    [SerializeField] protected PlayerAnimatorController player;
    [SerializeField] protected DanceStep playerDanceStep;
    
    [Header("FeedBack References")]
    [SerializeField] protected SpriteRenderer feedBack;

    private void Awake()
    {
        if (ActivateOnStart) ActivatePuzzle(true);
    }

    private DanceStep GetDanceStep(int counter, int counterCompass)
    {
        if (useCompass) return (DanceSteps.Count>0)?DanceSteps[counterCompass]:DanceStep.None;
        else
        {
            int aux = counter-1;
            if (ShouldRepeat) aux = counter % DanceSteps.Count;
            return (DanceSteps.Count>0)?DanceSteps[aux]:DanceStep.None;
        }
    }

    public virtual void ActivatePuzzle(bool activate)
    {
        isActive = activate;
        OnRhythmPuzzleStartedEvent?.Invoke();
    }
    
    public override void PreBeatAction(int counter, int counterCompass)
    {
        playerDanceStep = DanceStep.None;
        if (isActive)
        {
            currentPuzzleStep = GetDanceStep(counter, counterCompass);
            if(currentPuzzleStep!= DanceStep.None) canRecieveInput = true;
            OnPrepareStep?.Invoke(currentPuzzleStep);
        }
    }

    public override void BeatAction(int counter, int counterCompass)
    {
        if (isActive)
        {
            OnDanceStep?.Invoke(currentPuzzleStep);
            VisualFeedback(counter, counterCompass);
        }
    }

    public virtual void VisualFeedback(int counter, int counterCompass)
    {
        // To implement in Sons.
    }

    public override void PostBeatAction(int counter, int counterCompass)
    {
        if (isActive)
        {
            OnReleaseStep?.Invoke(currentPuzzleStep);
            if (!hasRecieveInput)PlayerDanceCheck();
        }
        canRecieveInput = false;
        hasRecieveInput = false;
        playerDanceStep = DanceStep.None;
    }
    
    private void PlayerDanceCheck()
    {
        if (currentPuzzleStep != DanceStep.None && player != null)
        {
            Debug.Log("Checking Dance: " + ((currentPuzzleStep==playerDanceStep)?"Succes":"Failed"));
            PlayerDanceReaction(currentPuzzleStep==playerDanceStep);
        }
    }

    public virtual void PlayerDanceReaction(bool IsPlayerDanceCorrect)
    {
        ///To implement in sons.
    }
    

    public virtual void OnPlayerInputAction(DanceStep step)
    {
        if (isActive && canRecieveInput && !hasRecieveInput)
        {
            playerDanceStep = step;
            hasRecieveInput = true;
            PlayerDanceCheck();
        }
    }

    public void OnRhythmPuzzleStarted()
    {
        OnRhythmPuzzleStartedEvent?.Invoke();
    }

    public void OnRhythmPuzzleCompleted()
    {
        OnRhythmPuzzleCompletedEvent?.Invoke();
    }
    public void OnRhythmPuzzleFailed()
    {
        OnRhythmPuzzleFailedEvent?.Invoke();
    }
    
}
