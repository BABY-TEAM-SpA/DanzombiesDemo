using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DamageMode
{
    None,
    ModificaFlow,
    ModificaFlowYDaña
}


public class DanceZone : Dancer
{
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;
    [SerializeField] private bool isActive;
    private RhythmPuzzle puzzle;
    [Header("Dance Zone Settings")]
    [SerializeField] private DamageMode damageMode;
    [SerializeField] private List<Dancer> dancers = new List<Dancer>();
    public DanceEventManager listeners = new DanceEventManager();
    
    [Header("Players")] 
    protected bool PlayerHasDanced=false;
    
    private BeatManager.BeatType compareBeatType;
    
    
    public PlayerManager playersInside{private set; get;}


    
    
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
        if(dancers.Count>0)foreach (Dancer dancer in dancers) listeners.AddListener(dancer);
    }
    private void OnDisable()
    {
        listeners.RemoveAllListeners();
    }

    public override void OnEnablePuzzle(RhythmPuzzle puz)
    {
        //Debug.Log("OnEnablePuzzle");
        OnActivated?.Invoke();
        isActive = true;
        puzzle = puz;
    }

    public override void OnDisablePuzzle(RhythmPuzzle puz)
    {
        //Debug.Log("OnDisablePuzzle");
        isActive = false;
        OnDeactivated?.Invoke();
    }
    
    public override void OnPrepareStepAction(int prevbeat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if (!isActive) return;
        PlayerHasDanced = false;
        currentBeat = BeatManager.Instance? BeatManager.Instance.globalBeatCount+1:1;
        currentBeatType = beatType;
        base.OnPrepareStepAction(prevbeat,beatType, danceStep);
    }
    
    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if (!isActive) return;
        currentBeat = BeatManager.Instance? BeatManager.Instance.globalBeatCount:1;
        listeners.InvokeDance(beat,beatType, danceStep);
        onDance?.Invoke(danceStep);
    }

    public override void OnReleaseStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if (!isActive) return;
        if (playersInside!= null &&!PlayerHasDanced && danceStep != DanceStep.None && danceStep != DanceStep.Idle)
        {
            //Debug.Log("didntDance");
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
        foreach (Dancer dancer in dancers)
            dancer.React(exp);
        
    }
    public void SetPlayerInput(DanceStep step, out BeatReciever.BeatFeedback bf)
    {
        bf = BeatReciever.BeatFeedback.Ignored;
        if (!isActive) return;
        if (PlayerHasDanced) return;
        else
        {
            PlayerHasDanced = true;
            bool isTheSameStep = step == currentDanceStep;
            //Debug.Log(isTheSameStep);
            bf = isTheSameStep ? BeatManager.Instance.EvaluateInput(currentBeat,currentBeatType) : BeatReciever.BeatFeedback.Bad;
            React(bf==BeatReciever.BeatFeedback.Bad?ExpressionType.Angry:ExpressionType.Normal);
            puzzle?.ResolvePlayerInput(bf);
        }
        
    }

    public void RefreshZombies()
    {
        dancers.Clear();

        foreach (Transform child in transform)
            if (child.TryGetComponent<ZombieDanceBrain>(out ZombieDanceBrain zombie))
                dancers.Add(zombie);
    }
}