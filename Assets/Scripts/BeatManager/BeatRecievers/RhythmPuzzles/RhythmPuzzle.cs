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

public enum DanceResult
{
    Neutral,
    Succes,
    Failed
}

[Serializable]
public class SequenceStep
{
    
    [SerializeField] public List<DanceStep> DanceSteps = new List<DanceStep>();
}

public class RhythmPuzzle : BeatReciever
{
    protected bool isActive = false;
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
    
    [Header("FeedBack References")]
    [SerializeField] protected SpriteRenderer feedBack;
    
    [Header("Players")]
    protected List<PlayerManager> playersInside = new List<PlayerManager>();

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
        if (isActive)
        {
            currentPuzzleStep = GetDanceStep(counter, counterCompass);
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
    public override void PostBeatAction(int counter, int counterCompass)
    {
        if (isActive)
        {
            OnReleaseStep?.Invoke(currentPuzzleStep);
        }
    }

    public virtual void VisualFeedback(int counter, int counterCompass)
    {
        // To implement in Sons
    }

    public virtual void OnPlayerInputAction(DanceStep step)
    {
        //To implement in Sons
    }
    
    public DanceResult CheckPlayerDance(DanceStep playerStep)
    {
        if (isActive&& currentPuzzleStep != DanceStep.None)
        {
            if (currentPuzzleStep == playerStep)
            {
                return DanceResult.Succes;
            }
            else
            {
                return DanceResult.Failed;
            }
        }
        return DanceResult.Neutral;
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

    public virtual void PlayerEnter(PlayerManager player)
    {
        Debug.Log("Player entered");
        player.AddTargetPuzzle(this);
        playersInside.Add(player);
        //To implement in Sons
    }

    public virtual void PlayerLeave(PlayerManager player)
    {
        player.RemoveTargetPuzzle(this);
        playersInside.Remove(player);
        //To implement in Sons
    }
}
