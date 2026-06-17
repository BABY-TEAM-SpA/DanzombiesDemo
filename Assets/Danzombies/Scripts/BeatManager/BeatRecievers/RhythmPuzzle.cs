using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class RhythmPuzzle : BeatReciever
{
    public enum RhythmSyncMode  { Global, Local}
    
    [Header("Rhythm Puzzle Settings")]
    [SerializeField] protected bool debug;
    [SerializeField] bool activateOnStart;
    [SerializeField] RhythmSyncMode syncMode = RhythmSyncMode.Global;
    
    protected SequenceStep CurrentSequence;
    protected DanceStep CurrentPuzzleStep = DanceStep.None;
    protected DanceStep NextPuzzleStep = DanceStep.None;
    protected int InnerCounter;
    private int startBeat;
    
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public delegate void OnMusicEvent2(DanceStep danceStep, DanceStep futureStep);
    public event OnMusicEvent2 OnReleaseStep;
    public UnityEvent onPuzzleGetsActivateEvent = new UnityEvent();

    [Header("Players")] 
    protected bool PlayerHasDanced=false;
    [SerializeField] protected PlayerManager playersInside;
    
    private void Start()
    {
        PreparePuzzle();
        if (activateOnStart) ActivatePuzzle(true);
    }

    protected void SetSequence(SequenceStep sequence)
    {
        CurrentSequence = sequence;
        CurrentSequence.startCounter = InnerCounter;
        beatType = CurrentSequence.patternBeatType;
    }


    // Tested - Working
    public virtual bool SetPlayerInput(DanceStep playerStep, out BeatFeedback bf)
    {
        bool isCorrect = playerStep == CurrentPuzzleStep;
        bf = isCorrect? BeatManager.Instance.EvaluateInput(beatType): BeatFeedback.Bad;
        if (!PlayerHasDanced && isOnBeat)
        {
            PlayerHasDanced = true;
            CurrentSequence.ApplyDance(isCorrect);
            return true;
        }
        return false;
    }
    
    public abstract void PreparePuzzle();
    
    public virtual void ActivatePuzzle(bool activate)
    {
        isActive = activate;

        if (!activate)
            return;

        if (syncMode == RhythmSyncMode.Local)  startBeat = BeatManager.Instance.GetCounter(beatType);
        else startBeat = 0;

        onPuzzleGetsActivateEvent?.Invoke();
    }
    
    public override void PreBeatAction(int counter)
    {
        PlayerHasDanced = false;
        UpdateInnerCounter();
        CurrentSequence.GetDanceStep(InnerCounter, out CurrentPuzzleStep);
        if(debug) Debug.Log("___Puzzle PreBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PuzzlePreBeat();
        OnPrepareStep?.Invoke(CurrentPuzzleStep);
    }

    public override void BeatAction(int counter)
    {
        if(debug) Debug.Log("_____Puzzle Beat make "+CurrentPuzzleStep.ToString()+" at "+counter+" on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PuzzleBeat();
        OnDanceStep?.Invoke(CurrentPuzzleStep);
        
    }
    public override void PostBeatAction(int counter)
    {
        if(debug) Debug.Log("___Puzzle PostBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PuzzlePostBeat();
        Action<RhythmPuzzle> callback = CurrentSequence.GetNextDanceStep(InnerCounter, out NextPuzzleStep);
        callback?.Invoke(this);
        OnReleaseStep?.Invoke(CurrentPuzzleStep,NextPuzzleStep);
        CheckPlayerPost();
        CurrentPuzzleStep = DanceStep.None;
        PlayerHasDanced = false;
    }

    protected virtual void CheckPlayerPost()
    {
        if (!PlayerHasDanced && playersInside != null && CurrentPuzzleStep != DanceStep.None)
        {
            CurrentSequence.ApplyDance(false);
            playersInside.ReportEmptyDance();
        }
    }
    
    protected virtual void PlayerEnter(PlayerManager player)
    {
        if(debug)Debug.Log("Player entered");
        player.AddTargetPuzzle(this);
        playersInside = player;
    }
    
    protected virtual void PlayerLeave(PlayerManager player)
    {
        if(debug)Debug.Log("Player Leave");
        playersInside = null;
        player.RemoveTargetPuzzle(this);
    }

    public abstract void PlayerGetDamaged();
    
    void UpdateInnerCounter()
    {
        int globalBeat = BeatManager.Instance.GetCounter(beatType);

        if (syncMode == RhythmSyncMode.Local) InnerCounter = globalBeat - startBeat;
        else InnerCounter = globalBeat;

        if (InnerCounter < 0) InnerCounter = 0;
    }

    protected abstract void PuzzlePreBeat();
    protected abstract void PuzzleBeat();
    protected abstract void PuzzlePostBeat();

    public abstract void OnSequenceEnd();

}
