using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DamageMode
{
    None,
    ModificaFlow,
    ModificaFlowYDaña
}


public class DanceZone : Dancer
{
    [SerializeField] private RhythmPuzzle puzzle;
    [Header("Dance Zone Settings")]
    [SerializeField] private List<Dancer> dancers = new List<Dancer>();
    public DanceEventManager listeners = new DanceEventManager();
    
    [Header("Players")] 
    protected bool PlayerHasDanced=false;
    
    private BeatManager.BeatType compareBeatType;
    
    
    public PlayerManager playersInside{private set; get;}

    [SerializeField]
    private DamageMode damageMode;
    
    public DamageMode GetDamageMode()
    {
        return damageMode;
    }

    public void Start()
    {
        if(dancers.Count > 0) SetZone();
    }

    public void SetZone()
    {
        foreach (ZombieDanceBrain dancer in dancers) listeners.AddListener(dancer);
    }
    private void OnDisable()
    {
        listeners.RemoveAllListeners();
    }
    
    public override void OnPrepareStepAction(int prevbeat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        PlayerHasDanced = false;
        currentBeat = BeatManager.Instance.globalBeatCount+1;
        currentBeatType = beatType;
        base.OnPrepareStepAction(prevbeat,beatType, danceStep);
    }
    
    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        currentBeat = BeatManager.Instance.globalBeatCount;
        listeners.InvokeDance(beat,beatType, danceStep);
        onDance?.Invoke(danceStep);
    }

    public override void OnReleaseStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if (playersInside!= null &&!PlayerHasDanced && danceStep != DanceStep.None && danceStep != DanceStep.Idle)
        {
            Debug.Log("didntDance");
            playersInside?.ApplyDanceFeedback(BeatReciever.BeatFeedback.Bad);
        }
        base.OnReleaseStepAction(beat,beatType, danceStep);
    }
    

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
                PlayerEnter(player);
        }
    public void PlayerEnter(PlayerManager player)
    {
        player.AddTargetPuzzle(this);
        playersInside = player;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
            PlayerLeave(player);
    }
    public virtual void PlayerLeave(PlayerManager player)
    {
        playersInside = null;
        player.RemoveTargetPuzzle(this);
    }

    
    
    public void React(ExpressionType exp)
    {
        foreach (ZombieDanceBrain dancer in dancers)
            dancer.React(exp);
        
    }
    public void SetPlayerInput(DanceStep step, out BeatReciever.BeatFeedback bf)
    {
        
        bf = BeatReciever.BeatFeedback.Bad;
        if (PlayerHasDanced) return;
        else
        {
            PlayerHasDanced = true;
            bool isTheSameStep = step == currentDanceStep;
            //Debug.Log(isTheSameStep);
            bf = isTheSameStep ? BeatManager.Instance.EvaluateInput(currentBeat,currentBeatType) : BeatReciever.BeatFeedback.Bad;
            React(bf==BeatReciever.BeatFeedback.Bad?ExpressionType.Angry:ExpressionType.Normal);
        }
        
    }
}