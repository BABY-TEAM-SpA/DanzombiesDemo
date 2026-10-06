using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DamageMode { None, ModificaFlow, ModificaFlowYDaña }

public class DanceZone : Dancer
{
    #region [VARIABLES]
    [SerializeField] private bool isActive;

    [Header("Dance Zone - Settings")]
    [SerializeField] public DamageMode damageMode;
    [SerializeField] private List<Dancer> dancers = new List<Dancer>();

    public DanceEventManager listeners = new DanceEventManager();
    
    [Header("Players")] 
    protected bool PlayerHasDanced;

    public PlayerManager playersInside { private set; get; }
    public DamageMode DamageMode => damageMode;

    private RhythmPuzzle puzzle;
    private BeatManager.BeatType compareBeatType;

    [Header("Dance Zone - Events")]
    public UnityEvent OnPlayerEntered;
    public UnityEvent OnPlayerExited;

    public Action<BeatReciever.BeatFeedback> OnPlayerFeedback; // <- [Frco] Único evaluador de la entrada del jugador.
    // El FlowComponent y PlayerManager escuchan este evento para reaccionar a la entrada del jugador.
    #endregion

    #region [UNITY]
    public void Start()
    {
        if (dancers.Count > 0)
            SetZone();
    }
    private void OnDisable() => listeners.RemoveAllListeners();

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
            PlayerEnter(player);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
            PlayerLeave(player);
    }
    #endregion
    #endregion

    #region [METHODS]
    #region API
    // [Frco] <¬ Para poder añadir/remover un Dancer de una DanceZone desde un UnityEvent
    public void AddListener(Dancer dancer)
    {
        if (dancer != null)
            listeners.AddListener(dancer);
    }
    public void RemoveListener(Dancer dancer)
    {
        if (dancer != null)
            listeners.RemoveListener(dancer);
    }

    public void SetZone()
    {
        if (dancers.Count > 0)
            foreach (Dancer dancer in dancers)
                listeners.AddListener(dancer);
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
            bf = isTheSameStep ? BeatManager.Instance.EvaluateInput(currentBeat, currentBeatType) : BeatReciever.BeatFeedback.Bad;
            React(bf == BeatReciever.BeatFeedback.Bad ? ExpressionType.Angry : ExpressionType.Normal);
            ReactToFeedback(bf);
        }
    }
    #endregion

    #region Enter/Exit
    public void PlayerEnter(PlayerManager player)
    {
        player.AddTargetPuzzle(this);
        playersInside = player;
        OnPlayerEntered?.Invoke();
    }

    public virtual void PlayerLeave(PlayerManager player)
    {
        playersInside = null;
        player.RemoveTargetPuzzle(this);
        OnPlayerExited?.Invoke();
    }
    #endregion

    #region Dancer - Enable/Disable
    public override void OnEnablePuzzle(RhythmPuzzle puz)
    {
        base.OnEnablePuzzle(puz);
        isActive = true;
        puzzle = puz;
        listeners.InvokeEnablePuzzle(puz);
    }

    public override void OnDisablePuzzle(RhythmPuzzle puz)
    {
        base.OnDisablePuzzle(puz);
        isActive = false;
        listeners.InvokeDisablePuzzle(puz);
    }
    #endregion

    #region Dancer - Step Actions
    public override void OnPreDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        listeners.InvokePreDance(beat, beatType);
    }

    public override void OnPrepareStepAction(int prevbeat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if (!isActive) return;
        PlayerHasDanced = false;
        currentBeat = BeatManager.Instance ? BeatManager.Instance.globalBeatCount + 1 : 1;
        currentBeatType = beatType;
        base.OnPrepareStepAction(prevbeat, beatType, danceStep);
        listeners.InvokePrepare(prevbeat, beatType, danceStep);
    }

    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if (!isActive) return;
        currentBeat = BeatManager.Instance ? BeatManager.Instance.globalBeatCount : 1;
        listeners.InvokeDance(beat, beatType, danceStep);
        onDance?.Invoke(danceStep);
    }

    public override void OnReleaseStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if (!isActive) return;
        if (playersInside != null && !PlayerHasDanced && danceStep != DanceStep.None)
            ReactToFeedback(BeatReciever.BeatFeedback.Bad); // <- [Frco] ¿No debería ser Ignored?
            //playersInside?.ApplyDanceFeedback(BeatReciever.BeatFeedback.Bad);
        base.OnReleaseStepAction(beat, beatType, danceStep);
        listeners.InvokeRealease(beat, beatType, danceStep);
    }

    public override void OnSetNextSetAction(int nextBeat, BeatManager.BeatType beatType, DanceStep nextDanceStep)
    {
        listeners.InvokeNextStep(nextBeat, beatType, nextDanceStep);
    }
    #endregion

    #region Helpers
    private void ReactToFeedback(BeatReciever.BeatFeedback bf)
    {
        puzzle?.ResolvePlayerInput(bf);
        playersInside?.ApplyDanceFeedback(bf);
        OnPlayerFeedback?.Invoke(bf);
    }

    public void RefreshZombies()
    {
        dancers.Clear();

        foreach (Transform child in transform)
            if (child.TryGetComponent<ZombieDanceBrain>(out ZombieDanceBrain zombie))
                dancers.Add(zombie);
        foreach (Dancer dancer in dancers)
            if (TryGetComponent<Position3D>(out Position3D pos))
                pos.SetLayerOnSprites(); ;
    }
    #endregion
    #endregion
}