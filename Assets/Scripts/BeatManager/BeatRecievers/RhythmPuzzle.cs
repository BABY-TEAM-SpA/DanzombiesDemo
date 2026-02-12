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
    [SerializeField] protected bool ShouldRepeat =false;
    [SerializeField] protected List<DanceStep> DanceSteps = new List<DanceStep>();
    [SerializeField] protected DanceStep currentPuzzleStep = DanceStep.None;
    
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public event OnMusicEvent OnReleaseStep;
    public UnityEvent OnRhythmPuzzleStartedEvent = new UnityEvent();
    public UnityEvent OnRhythmPuzzleCompletedEvent = new UnityEvent();
    
    
    [Header("FeedBack References")]
    [SerializeField] protected SpriteRenderer feedBack;
    
    [Header("Players")]
    protected List<PlayerManager> playersInside = new List<PlayerManager>();
    
    [Header("Admin")]
    [SerializeField] protected bool debug;

    private void Awake()
    {
        if (ActivateOnStart) ActivatePuzzle(true);
    }

    private DanceStep GetDanceStep(int counter)
    {
        // recibi un -1
        if(counter<0) return DanceStep.None;
        if (counter < DanceSteps.Count)
        {
            return DanceSteps[counter];
        }
        else{
            return (ShouldRepeat)?DanceSteps[(counter) % DanceSteps.Count]:DanceStep.None;
        }
        //si esta dentro
        //si esta fuera
        //si debo repetir y esta dentro
        //si debo repetir y esta fuera 
    }

    public virtual void ActivatePuzzle(bool activate)
    {
        isActive = activate;
        OnRhythmPuzzleStartedEvent?.Invoke();
    }
    
    public override void PreBeatAction(int counter)
    {
        if (isActive)
        {
            currentPuzzleStep = GetDanceStep(counter);
            OnPrepareStep?.Invoke(currentPuzzleStep);
        }
    }

    public override void BeatAction(int counter)
    {
        if (isActive)
        {
            if(debug)Debug.Log("______Puzzle make "+currentPuzzleStep.ToString()+" at "+counter+" on "+AudioSettings.dspTime.ToString());
            OnDanceStep?.Invoke(currentPuzzleStep);
            VisualFeedback(counter);
        }
    }
    public override void PostBeatAction(int counter)
    {
        if (isActive)
        {

            //Debug.Log("PostBeat");
            OnReleaseStep?.Invoke(currentPuzzleStep);
            OnRhythmPuzzleBeatReaction();
            
        }
    }

    public virtual void VisualFeedback(int counter)
    {
        // To implement in Sons
    }

    public virtual void OnPlayerInputAction(DanceStep step)
    {
        //To implement in Sons
    }

    public void OnRhythmPuzzleStarted()
    {
        OnRhythmPuzzleStartedEvent?.Invoke();
    }

    public void OnRhythmPuzzleCompleted()
    {
        OnRhythmPuzzleCompletedEvent?.Invoke();
        
    }
    public virtual void OnRhythmPuzzleBeatReaction()
    {
        
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
        Debug.Log("Player Leave");
        playersInside.Remove(player);

        player.RemoveTargetPuzzle(this);
        
        //To implement in Sons
    }
    
}
