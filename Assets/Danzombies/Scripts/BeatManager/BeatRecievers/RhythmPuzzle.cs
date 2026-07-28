using System;
using UnityEngine;
using UnityEngine.Events;

public class EventChannel
{
    private delegate void OnMusicEvent(DanceStep danceStep, DanceStep nextDanceStep=DanceStep.None);
    private event OnMusicEvent OnPrepareStep;
    private event OnMusicEvent OnDanceStep;
    private event OnMusicEvent OnReleaseStep;
    

    public void AddListener(ZombieDanceBrain zombie)
    {
        OnPrepareStep += zombie.OnPrepareStepAction;
        OnDanceStep += zombie.OnDanceStepAction;
        OnReleaseStep += zombie.OnReleaseStepAction;
    }
    public void RemoveListener(ZombieDanceBrain zombie)
    {
        OnPrepareStep -= zombie.OnPrepareStepAction;
        OnDanceStep -= zombie.OnDanceStepAction;
        OnReleaseStep -= zombie.OnReleaseStepAction;
    }
    public void RemoveAllListeners()
    {
        OnPrepareStep = null;
        OnDanceStep = null;
        OnReleaseStep = null;
    }

    public void InvokePrepare(DanceStep danceStep, DanceStep nextDanceStep)
    {
        OnPrepareStep?.Invoke(danceStep, nextDanceStep);
    }
    public void InvokeDance(DanceStep danceStep, DanceStep nextDanceStep= DanceStep.None)
    {
        OnDanceStep?.Invoke(danceStep, nextDanceStep);
    }

    public void InvokeRealease(DanceStep danceStep, DanceStep nextDanceStep)
    {
        OnReleaseStep?.Invoke(danceStep, nextDanceStep);
    }
}

[Serializable]
public class DanceData
{
    public SequenceStep Sequence = new SequenceStep();
    public DanceStep DanceStep = DanceStep.None;
    public DanceStep NextDanceStep = DanceStep.None;
    public EventChannel listeners = new EventChannel();
    
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
    
    protected DanceData currentDanceData = new DanceData();
    protected DancerExpression.ExpressionType currentReaction= DancerExpression.ExpressionType.Normal;
    

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
        SetCounter(0);
        currentDanceData.Sequence = sequence;
        currentDanceData.Sequence.OnDanceSequenceFinished.AddListener(() => { OnDanceSequenceCleared(); });
        beatType = currentDanceData.Sequence.patternBeatType;
    }

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

    protected virtual void PlayerLeave(PlayerManager player)
    {
        if (debug)
            Debug.Log("Player Leave");

        playersInside = null;
        player.RemoveTargetPuzzle(this);
    }

    public virtual void ReactToPlayerStatus(DancerExpression.ExpressionType exp){}
    public virtual void PlayerGetDamaged(){}
    

    public SequenceStep.DamageMode GetDamageMode()
    {
        return currentDanceData.Sequence.damageMode;
    }
    
    public abstract void OnDanceSequenceCleared();

}
