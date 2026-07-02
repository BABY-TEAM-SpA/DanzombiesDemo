using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DanceData
{
    public SequenceStep Sequence;
    public DanceStep DanceStep = DanceStep.None;
    public DanceStep NextDanceStep = DanceStep.None;
}

public abstract class RhythmPuzzle : BeatReciever
{
    public enum RhythmSyncMode  { Global, Local}
    
    [Header("Rhythm Puzzle Settings")]
    [SerializeField] protected bool debug;
    [SerializeField] bool activateOnStart;
    [SerializeField] RhythmSyncMode syncMode = RhythmSyncMode.Global;
    
    
    [HideInInspector] public DanceData currentDanceData;
    protected DancerExpression.ExpressionType currentReaction= DancerExpression.ExpressionType.Normal;
    protected int InnerCounter = 0;
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
        currentDanceData.Sequence = sequence;
        currentDanceData.Sequence.startCounter = InnerCounter;
        beatType = currentDanceData.Sequence.patternBeatType;
    }


    // Tested - Working
    public virtual bool SetPlayerInput(DanceStep playerStep, out BeatFeedback bf)
    {
        bool isCorrect = playerStep == currentDanceData.DanceStep;
        bf = isCorrect? BeatManager.Instance.EvaluateInput(beatType): BeatFeedback.Bad;
        if (!PlayerHasDanced && isOnBeat)
        {
            PlayerHasDanced = true;
            currentDanceData.Sequence.ApplyDance(isCorrect);
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
        currentDanceData.Sequence.GetDanceStep(InnerCounter, out currentDanceData.DanceStep);
        if(debug) Debug.Log("___Puzzle PreBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PuzzlePreBeat();
        OnPrepareStep?.Invoke(currentDanceData.DanceStep);
    }

    public override void BeatAction(int counter)
    {
        if(debug) Debug.Log("_____Puzzle Beat make "+currentDanceData.DanceStep.ToString()+" at "+counter+" on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PuzzleBeat();
        OnDanceStep?.Invoke(currentDanceData.DanceStep);
        
    }
    public override void PostBeatAction(int counter)
    {
        if(debug) Debug.Log("___Puzzle PostBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PuzzlePostBeat();
        Action<RhythmPuzzle> callback = currentDanceData.Sequence.GetNextDanceStep(InnerCounter, out currentDanceData.NextDanceStep);
        callback?.Invoke(this);
        OnReleaseStep?.Invoke(currentDanceData.DanceStep,currentDanceData.NextDanceStep);
        CheckPlayerPost();
        currentDanceData.DanceStep = DanceStep.None;
        PlayerHasDanced = false;
    }

    protected virtual void CheckPlayerPost()
    {
        if (!PlayerHasDanced && playersInside != null && currentDanceData.DanceStep != DanceStep.None)
        {
            currentDanceData.Sequence.ApplyDance(false);
            playersInside.ReportEmptyDance();
        }
    }
    
    protected virtual void PlayerEnter(PlayerManager player)
    {
        if(debug)Debug.Log("Player entered");
        player.AddTargetPuzzle(this);
        playersInside = player;
    }

    public abstract void ReactToPlayerStatus(DancerExpression.ExpressionType exp);

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
