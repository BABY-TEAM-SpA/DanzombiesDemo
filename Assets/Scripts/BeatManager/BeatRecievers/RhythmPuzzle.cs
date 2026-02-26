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
    public UnityEvent OnSequenceCompletedEvent;
}

public abstract class RhythmPuzzle : BeatReciever
{
    [SerializeField] bool ActivateOnStart;
    [SerializeField] protected bool ShouldRepeat =false;
    [SerializeField] protected SequenceStep currentDanceSequence;
    [SerializeField] protected DanceStep currentPuzzleStep = DanceStep.None;
    
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public event OnMusicEvent OnReleaseStep;
    public UnityEvent OnPuzzleGetsActivateEvent = new UnityEvent();
    
    
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
        if (counter < currentDanceSequence.DanceSteps.Count)
        {
            return currentDanceSequence.DanceSteps[counter];
        }
        else{
            return (ShouldRepeat)?currentDanceSequence.DanceSteps[(counter) % currentDanceSequence.DanceSteps.Count]:DanceStep.None;
        }
        //si esta dentro
        //si esta fuera
        //si debo repetir y esta dentro
        //si debo repetir y esta fuera 
    }

    public virtual void ActivatePuzzle(bool activate)
    {
        isActive = activate;
        if(activate)OnPuzzleGetsActivateEvent?.Invoke();
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
            GeneralVisualFeedback(counter);
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

    public void OnRhythmPuzzleBeatReaction()
    {
        if(playersInside.Count>0 && currentPuzzleStep != DanceStep.None){
            List<PlayerManager> players = new List<PlayerManager>(playersInside);
            bool anyPlayerIsCorrect = false;
            foreach (PlayerManager player in players)
            {
                ReactToPlayersDance(player, currentPuzzleStep);
            }
            VisualFeedbackToPlayerDance(anyPlayerIsCorrect);
        }
    }

    public abstract void ReactToPlayersDance(PlayerManager player,DanceStep step);
    
    public abstract void VisualFeedbackToPlayerDance(bool isPlayerDanceCorrect);

    public abstract void GeneralVisualFeedback(int counter);

    public abstract void PlayerHasNoFlow(PlayerManager player);
    
    public virtual void PlayerEnter(PlayerManager player)
    {
        if(debug)Debug.Log("Player entered");
        player.AddTargetPuzzle(this);
        playersInside.Add(player);
    }

    public virtual void PlayerLeave(PlayerManager player)
    {
        if(debug)Debug.Log("Player Leave");
        playersInside.Remove(player);
        player.RemoveTargetPuzzle(this);
    }
    
}
