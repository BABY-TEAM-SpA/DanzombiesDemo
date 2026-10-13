using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DamageMode { None, ModificaFlow, ModificaFlowYDaña }


public class StepRegist
{
    public DanceStep danceStep;
    public int stepTimeMs;
    public int life;

    public StepRegist(DanceStep step, int timeMs)
    {
        danceStep = step;
        stepTimeMs = timeMs;
    }
}

[Serializable]
public class PlayerInputEvent
{
    public BeatReciever.BeatFeedback feedback;
    public UnityEvent OnPlayerSuccess;
}

public class DanceZone : Dancer
{
    #region [VARIABLES]
    [SerializeField] private bool isActive;

    [Header("Dance Zone - Settings")]
    [SerializeField] private DamageMode damageMode;
    [SerializeField] private List<Dancer> dancers = new List<Dancer>();
    DanceEventBuffer danceEventBuffer = new DanceEventBuffer();
    private Queue<StepRegist> danceStepsInputRegists = new Queue<StepRegist>();
    
    [Header("Players")] 
    public PlayerManager playersInside { private set; get; }
    private Queue<StepRegist> playerInputRegists = new Queue<StepRegist>();
    
    [SerializeField] protected PlayerInputEvent[] playerInputs;
    public DamageMode DamageMode => damageMode;

    private RhythmPuzzle puzzle;

    [Header("Dance Zone - Events")]
    public UnityEvent OnPlayerEntered;
    public UnityEvent OnPlayerExited;

    public Action<BeatReciever.BeatFeedback> OnPlayerFeedback; // <- [Frco] Único evaluador de la entrada del jugador.
                                                               // El FlowComponent escucha este evento para reaccionar a la entrada del jugador.
    #endregion

    #region [UNITY]
    public void Start()
    {
        if (dancers.Count > 0)
            SetZone();
    }
    
    
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
    #region API
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

    public void SetZone()
    {
        if (dancers.Count > 0) 
            foreach (Dancer dancer in dancers) 
                danceEventBuffer.AddListener(dancer);
    }

    public void React(ExpressionType exp)
    {
        foreach (Dancer dancer in dancers) dancer.React(exp);
    }

    public void SetPlayerInput(int beatTimeMs ,DanceStep step, out BeatReciever.BeatFeedback bf)
    {
        bf = BeatReciever.BeatFeedback.Ignored;
        if (!isActive) return;
        else
        {
            StepRegist input = new StepRegist(step,beatTimeMs);
            playerInputRegists.Enqueue(input);
            bf = TryCheckPlayerZone();
        }
    }

    private BeatReciever.BeatFeedback TryCheckPlayerZone()
    {
        BeatReciever.BeatFeedback bf = BeatReciever.BeatFeedback.Ignored;
        if(danceStepsInputRegists.TryPeek(out StepRegist zone) && playerInputRegists.TryPeek(out StepRegist player))
        {
            danceStepsInputRegists.Dequeue();
            playerInputRegists.Dequeue();
            bf = player.danceStep == zone.danceStep ? EvaluateInput(zone.stepTimeMs,player.stepTimeMs) : BeatReciever.BeatFeedback.Bad;
            //React(bf == BeatReciever.BeatFeedback.Bad ? ExpressionType.Angry : ExpressionType.Normal);
            //ReactToFeedback(bf);
        }
        return bf;
    }

    public void CompleteZone()
    {
        Debug.Log($"[DanceZone] {name} completed.");
        OnCompleted?.Invoke();
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
        danceEventBuffer.InvokeEnablePuzzle(puz);
    }

    public override void OnDisablePuzzle(RhythmPuzzle puz)
    {
        base.OnDisablePuzzle(puz);
        isActive = false;
        danceEventBuffer.InvokeDisablePuzzle(puz);
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
        
        ///PreCheck
        if (playersInside != null)
        {
            if (playerInputRegists.TryPeek(out StepRegist playerInput))
            {
                if ((playerInput.life - (int)beatType) > 2)
                {
                    Debug.Log($"killed in player queue; remains:{playerInputRegists.Count}");
                    playerInputRegists.Dequeue();
                }
            }
            if(danceStepsInputRegists.TryPeek(out StepRegist zone))
            {
                  if((zone.life - (int)beatType) > 2)
                  {
                      Debug.Log($"killed in queue; remains:{danceStepsInputRegists.Count}");
                      danceStepsInputRegists.Dequeue();
                      ReactToFeedback(BeatReciever.BeatFeedback.Bad);
                  }
            }
        }
        currentDanceStep = danceStep;
        danceEventBuffer.InvokeDance(beatCounter, beatType, beatTimeMs, danceStep);
        onDance?.Invoke(danceStep);
        if (playersInside != null && danceStep != DanceStep.None)
        {
            StepRegist zone =  new StepRegist(danceStep,beatTimeMs);
            playerInputRegists.Enqueue(zone);
            Debug.Log($"added: {zone}");
        }
        
        
    }

    public override void OnSetNextSetAction(int nextBeat, BeatManager.BeatType beatType, DanceStep nextDanceStep)
    {
        danceEventBuffer.InvokeNextStep(nextBeat, beatType, nextDanceStep);
    }
    #endregion

    #region Helpers
    private void ReactToFeedback(BeatReciever.BeatFeedback bf)
    {
        ResolvePlayerInput(bf);
        playersInside?.ApplyDanceFeedback(bf);
        OnPlayerFeedback?.Invoke(bf);
    }
    public void ResolvePlayerInput(BeatReciever.BeatFeedback fb)
    {
        Debug.Log("ResolvePlayerInput is broke");
        /*currentDanceSequence?.RegisterStepFeedback(fb);
        PlayerInputEvent e = playerInputs.FirstOrDefault(p => p.feedback == fb);
        e?.OnPlayerSuccess?.Invoke();
        */
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

    public BeatReciever.BeatFeedback EvaluateInput(int beatTime, int inputTime)
    {
        int beatLength = BeatManager.Instance.BeatDurationMs;
        int delta = beatTime - inputTime;
        
        delta = ((delta + beatLength / 2) % beatLength + beatLength) % beatLength - beatLength / 2;
        
        int absDelta = Math.Abs(delta);
        float maxWindow = beatLength * BeatManager.Instance.margenPercentOnBeat;
        float greatWindow = maxWindow * BeatManager.Instance.greatPercentOnMargin;
        float perfectWindow = greatWindow * BeatManager.Instance.perfectPercentOnMargin;
        if (absDelta <= perfectWindow) return BeatReciever.BeatFeedback.Perfect;
        if (absDelta <= greatWindow) return BeatReciever.BeatFeedback.Good;
        if (absDelta <= maxWindow) return BeatReciever.BeatFeedback.Ok;
        return BeatReciever.BeatFeedback.Bad;
    }
}