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
    public enum GoalType
    {
        Default,
        CorrectXTimes,
        CompleteFullPattern,
        FillFlow
    }
    public GoalType goalType;
    public bool flowAffect;
    [SerializeField] public List<DanceStep> DanceSteps = new List<DanceStep>();
    public UnityEvent OnSequenceCompletedEvent;
}

public abstract class RhythmPuzzle : BeatReciever
{
    public enum RhythmSyncMode
    {
        Global,
        Local
    }
    [Header("Admin")]
    [SerializeField] protected bool debug;
    [Header("Rhythm Puzzle Settings")]
    [SerializeField] bool ActivateOnStart;
    [SerializeField] RhythmSyncMode syncMode = RhythmSyncMode.Global;
    [HideInInspector]public SequenceStep activeDanceSequence;
    protected DanceStep currentPuzzleStep = DanceStep.None;
    protected DanceStep futurePuzzleStep = DanceStep.None;
    protected int innerCounter;
    private int startBeat;
    
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public delegate void OnMusicEvent2(DanceStep danceStep, DanceStep futureStep);
    public event OnMusicEvent2 OnReleaseStep;
    public UnityEvent OnPuzzleGetsActivateEvent = new UnityEvent();
    
    [Header("Players")]
    protected List<PlayerManager> playersInside = new List<PlayerManager>();
    
    [Header("FeedBack References")]
    [SerializeField] protected SpriteRenderer feedBack;
    
    private void Awake()
    {
        if (ActivateOnStart) ActivatePuzzle(true);
    }

    private DanceStep GetDanceStep()
    {
        if(activeDanceSequence.DanceSteps.Count==0 || innerCounter<0) return DanceStep.None;
        if (innerCounter < activeDanceSequence.DanceSteps.Count)
        {
            return activeDanceSequence.DanceSteps[innerCounter];
        }
        else return DanceStep.None;
    }

    private DanceStep GetNextDanceStep() ///largo 4, estoy en el 49 (beat2), y el siguiente es en el 3 (beat4)
    {
        if(activeDanceSequence.DanceSteps.Count==0) return DanceStep.None;
        for (int i = 0; i < activeDanceSequence.DanceSteps.Count; i++)
        {
            int aux = i+innerCounter+1;
            aux = aux % activeDanceSequence.DanceSteps.Count;
            if(activeDanceSequence.DanceSteps[aux]!=DanceStep.None) return activeDanceSequence.DanceSteps[aux];
        }
        return DanceStep.None;
    }
    
    public virtual void ActivatePuzzle(bool activate)
    {
        isActive = activate;

        if (!activate)
            return;
        startBeat = 0;
        if(syncMode == RhythmSyncMode.Local)
            startBeat = AudioManager.Instance.SongPositionBeats();

        OnPuzzleGetsActivateEvent?.Invoke();
    }
    
    public override void PreBeatAction(int counter)
    {
        if (!isActive) return;

        UpdateInnerCounter();

        currentPuzzleStep = GetDanceStep();

        OnPrepareStep?.Invoke(currentPuzzleStep);
    }

    public override void BeatAction(int counter)
    {
        if (!isActive) return;
        if(debug)Debug.Log("______Puzzle make "+currentPuzzleStep.ToString()+" at "+counter+" on "+AudioSettings.dspTime.ToString());
        OnDanceStep?.Invoke(currentPuzzleStep);
        GeneralVisualFeedback(innerCounter);
        
    }
    public override void PostBeatAction(int counter)
    {
        if (!isActive) return;
        //Debug.Log("PostBeat");
        OnRhythmPuzzleBeatReaction();
        futurePuzzleStep = GetNextDanceStep();
        OnReleaseStep?.Invoke(currentPuzzleStep,futurePuzzleStep);
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
    
    void UpdateInnerCounter()
    {
        if (activeDanceSequence == null ||
            activeDanceSequence.DanceSteps.Count == 0)
        {
            innerCounter = 0;
            return;
        }

        int val= (AudioManager.Instance.SongPositionBeats()-startBeat) %
            activeDanceSequence.DanceSteps.Count;
        innerCounter = val>=0?val:0;
        
    }
}
