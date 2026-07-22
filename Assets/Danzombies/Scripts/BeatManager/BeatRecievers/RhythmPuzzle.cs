using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DanceData
{
    public SequenceStep Sequence = new SequenceStep();
    public DanceStep DanceStep = DanceStep.None;
    public DanceStep NextDanceStep = DanceStep.None;
    public void SetDanceStep(int beat)
    {
        DanceStep = Sequence.GetDanceStep(beat);
        
    }
    public void SetFutureDanceStep(int beat)
    {
        NextDanceStep = Sequence.GetFutureStep(beat);
    }
}

public abstract class RhythmPuzzle : BeatReciever
{
    #region [VARIABLES]
    [Header("Rhythm Puzzle Settings")]
    [SerializeField] protected bool debug;
    [SerializeField] bool activateOnStart;
    
    protected DanceData currentDanceData =new DanceData();
    protected DancerExpression.ExpressionType currentReaction= DancerExpression.ExpressionType.Normal;
    protected int InnerCounter = 0;
    private int startBeat;

    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;

    public delegate void OnMusicEvent2(DanceStep danceStep, DanceStep futureStep);
    public event OnMusicEvent2 OnReleaseStep;

    [Header("Players")] 
    protected bool PlayerHasDanced=false;
    public PlayerManager playersInside{private set; get;}
    
    private void Start()
    {
        PreparePuzzle();
        if (activateOnStart)
            ActivatePuzzle(true);
    }
    #endregion

    protected void SetSequence(SequenceStep sequence)
    {
        currentDanceData.Sequence = sequence;
        currentDanceData.Sequence.OnDanceSequenceFinished.AddListener(() => { OnDanceSequenceCleared(); });
        currentDanceData.Sequence.startCounter = InnerCounter;
        beatType = currentDanceData.Sequence.patternBeatType;
    }

    // Tested - Working
    public virtual bool SetPlayerInput(DanceStep playerStep, out BeatFeedback bf)
    {
        bool isCorrect = playerStep == currentDanceData.DanceStep;
        bf = isCorrect ? BeatManager.Instance.EvaluateInput(beatType) : BeatFeedback.Bad;
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

        //if (syncMode == RhythmSyncMode.Local) startBeat = BeatManager.Instance.GetCounter(beatType);
        startBeat = 0;
    }
    
    public override void PreBeatAction(int counter)
    {
        if(debug) Debug.Log("___Puzzle PreBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PlayerHasDanced = false;
        UpdateInnerCounter();
        currentDanceData?.SetDanceStep(InnerCounter);
        OnPrepareStep?.Invoke(currentDanceData.DanceStep);
    }

    public override void BeatAction(int counter)
    {
        if(debug) Debug.Log("_____Puzzle Beat make "+currentDanceData.DanceStep.ToString()+" at "+counter+" on "+AudioManager.Instance.SongPositionSeconds().ToString());
        OnDanceStep?.Invoke(currentDanceData.DanceStep);
        
    }
    public override void PostBeatAction(int counter)
    {
        if(debug) Debug.Log("___Puzzle PostBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        currentDanceData?.SetFutureDanceStep(InnerCounter);
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
        if (debug)
            Debug.Log("Player entered");

        player.AddTargetPuzzle(this);
        playersInside = player;
    }

    public abstract void ReactToPlayerStatus(DancerExpression.ExpressionType exp);

    protected virtual void PlayerLeave(PlayerManager player)
    {
        if (debug)
            Debug.Log("Player Leave");

        playersInside = null;
        player.RemoveTargetPuzzle(this);
    }

    public abstract void PlayerGetDamaged();
    
    void UpdateInnerCounter()
    {
        int globalBeat = BeatManager.Instance.GetCounter(beatType);
        InnerCounter = globalBeat;
    }

    public SequenceStep.DamageMode GetDamageMode()
    {
        return currentDanceData.Sequence.damageMode;
    }
    
    public abstract void OnDanceSequenceCleared();

}
