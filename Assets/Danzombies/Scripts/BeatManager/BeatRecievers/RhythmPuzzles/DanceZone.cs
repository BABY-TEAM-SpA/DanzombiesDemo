using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DamageMode { None, ModificaFlow, ModificaFlowYDaña }

public class DanceZone : Dancer
{
    #region [VARIABLES]
    #region Inspector
    [SerializeField] private bool isActive;
    [SerializeField] protected PlayerInputEvent[] playerInputs;

    [Header("Dance Zone - Settings")]
    public DamageMode DamageMode => damageMode;
    [SerializeField] private DamageMode damageMode;
    [SerializeField] private List<Dancer> dancers = new List<Dancer>();

    [Header("Dance Zone - Events")]
    public UnityEvent OnPlayerEntered;
    public UnityEvent OnPlayerExited;
    #endregion

    #region Interns
    public RhythmPuzzle puzzle { private set; get; }
    public PlayerManager playersInside { private set; get; }
    private DanceEventBuffer danceEventBuffer = new DanceEventBuffer(); // <- Dancers dentro de la DanceZone
    private DanceStepEvaluator evaluator = new DanceStepEvaluator();

    public Action<BeatReciever.BeatFeedback> OnPlayerFeedback; // <- [Frco] Único evaluador de la entrada del jugador.
                                                               // El FlowComponent escucha este evento para reaccionar a la entrada del jugador.
    #endregion
    #endregion

    #region [UNITY]
    public void Start()
    {
        if (dancers.Count > 0)
            SetZone();
    }

    private void OnEnable() => evaluator.OnFeedback += ReactToFeedback;
    private void OnDisable() => danceEventBuffer.RemoveAllListeners();

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
    #region API - Zone
    public void SetZone()
    {
        if (dancers.Count > 0) 
            foreach (Dancer dancer in dancers) 
                danceEventBuffer.AddListener(dancer);
    }

    public void React(ExpressionType exp)
    {
        foreach (Dancer dancer in dancers)
            dancer.React(exp);
    }

    public void CompleteZone()
    {
        Debug.Log($"[DanceZone] {name} completed.");
        OnCompleted?.Invoke();
    }
    #endregion

    #region API - Player Input
    public void SetPlayerInput(int beatTimeMs, DanceStep step, out BeatReciever.BeatFeedback bf)
    {
        bf = BeatReciever.BeatFeedback.Ignored;
        if (!isActive) return;
        evaluator.AddPlayer(step, beatTimeMs);
    }
    #endregion

    #region API - Enter/Exit
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

    #region API - Listeners
    // [Frco] <¬ Para poder añadir/remover un Dancer de una DanceZone desde un UnityEvent
    public void AddListener(Dancer dancer)
    {
        if (dancer != null)
            danceEventBuffer.AddListener(dancer);
    }
    public void RemoveListener(Dancer dancer)
    {
        if (dancer != null)
            danceEventBuffer.RemoveListener(dancer);
    }
    #endregion

    #region Dancer - Enable/Disable
    public override void OnEnablePuzzle(RhythmPuzzle puz)
    {
        base.OnEnablePuzzle(puz);
        isActive = true;
        puzzle = puz;
        danceEventBuffer.InvokeEnablePuzzle(puz);
    }

    public override void OnDisablePuzzle(RhythmPuzzle puz)
    {
        base.OnDisablePuzzle(puz);
        isActive = false;
        danceEventBuffer.InvokeDisablePuzzle(puz);
    }

    public override void OnReseatPuzzle(RhythmPuzzle puz)
    {
        base.OnReseatPuzzle(puz);
        evaluator.Clear();
    }
    #endregion

    #region Dancer - Step Actions
    public override void OnPreDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        danceEventBuffer.InvokePreDance(beat, beatType);
    }

    public override void OnDanceStepAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs, DanceStep danceStep)
    {
        if (!isActive) return;

        if (playersInside != null)
        {
            evaluator.Tick(beatTimeMs);
            if (danceStep != DanceStep.None)
                evaluator.AddExpected(danceStep, beatTimeMs);
        }

        currentDanceStep = danceStep;
        danceEventBuffer.InvokeDance(beatCounter, beatType, beatTimeMs, danceStep);
        onDance?.Invoke(danceStep);
    }

    public override void OnSetNextSetAction(int nextBeat, BeatManager.BeatType beatType, DanceStep nextDanceStep)
    {
        danceEventBuffer.InvokeNextStep(nextBeat, beatType, nextDanceStep);
    }
    #endregion

    #region Helpers
    private void ReactToFeedback(BeatReciever.BeatFeedback bf)
    {
        playersInside?.ApplyDanceFeedback(bf);
        puzzle.ReactToFeedback(bf);
        PlayerInputEvent e = playerInputs.FirstOrDefault(p => p.feedback == bf);
        e?.OnPlayerSuccess?.Invoke();
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
                pos.SetLayerOnSprites();
    }
    #endregion
    #endregion
}

[Serializable]
public class PlayerInputEvent
{
    public BeatReciever.BeatFeedback feedback;
    public UnityEvent OnPlayerSuccess;
}