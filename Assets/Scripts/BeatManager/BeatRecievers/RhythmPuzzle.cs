using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    internal bool currentDanceTriggered; //variable representa si el baile actual ha sido reaccionado ya por el jugador.
    //permite lanzar daño al jugador por no hacer ningun baile cuando deberia.
    
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public delegate void OnMusicEvent2(DanceStep danceStep, DanceStep futureStep);
    public event OnMusicEvent2 OnReleaseStep;
    public UnityEvent OnPuzzleGetsActivateEvent = new UnityEvent();
    public UnityEvent OnDanceUnreacted = new UnityEvent();
    
    /*[Header("Players")]
    protected List<PlayerManager> playersInside = new List<PlayerManager>(); //se vuelve innecesario por que ahora reaccionan a eventos de estos players.*/
    
    [Header("FeedBack References")]
    [SerializeField] protected SpriteRenderer feedBack;

    public Timing currentBeatTiming {private set; get;} //si queremos trackear grados de acierto (GREAT, GOOD, MISS) se necesitaria otra variable similar
    
    private void Start()
    {
        PreparePuzzle();
        if (ActivateOnStart) ActivatePuzzle(true);
    }
    public abstract void PreparePuzzle();
    private DanceStep GetDanceStep()
    {
        if(activeDanceSequence.DanceSteps.Count==0 || innerCounter<0) return DanceStep.None;
        int value = innerCounter % activeDanceSequence.DanceSteps.Count;
        return activeDanceSequence.DanceSteps[value];
    }

    private DanceStep GetNextDanceStep() ///largo 4, estoy en el 49 (beat2), y el siguiente es en el 3 (beat4)
    {
        if(activeDanceSequence.DanceSteps.Count==0 || innerCounter<0) return DanceStep.None;
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

        currentBeatTiming = Timing.Early;
    }

    public override void BeatAction(int counter)
    {
        if (!isActive) return;
        if(debug)Debug.Log("______Puzzle make "+currentPuzzleStep.ToString()+" at "+counter+ " ("+innerCounter+") on "+AudioSettings.dspTime.ToString());
        OnDanceStep?.Invoke(currentPuzzleStep);
        GeneralVisualFeedback(innerCounter);
        currentBeatTiming = Timing.Late;
        
    }
    public override void PostBeatAction(int counter)
    {
        if (!isActive) return;
        //Debug.Log("PostBeat");
        //OnRhythmPuzzleBeatReaction(); supersedido por onBeatWindow y chequeo por parte del PlayerManager
        futurePuzzleStep = GetNextDanceStep();
        OnReleaseStep?.Invoke(currentPuzzleStep,futurePuzzleStep);
        currentBeatTiming = Timing.Miss;
        if (!currentDanceTriggered && !(currentPuzzleStep == DanceStep.None)) //gatillar daño al player cuando no reacciona a un baile
        {
            OnDanceUnreacted?.Invoke();
        }
        currentDanceTriggered = false;
    }

    public void SubscribeToPlayerReactions(PlayerManager player) //llamado por PlayerManager cuando el puzzle se registra como activo
    {
        player.OnDanceEvent.AddListener(PlayerDanceReaction);
        OnDanceUnreacted.AddListener(player.MissedStep);
    }
    
    public void UnsubscribeToPlayerReactions(PlayerManager player) //llamado por PlayerManager cuando el puzzle se desregistra como activo
    {
        player.OnDanceEvent.RemoveListener(PlayerDanceReaction);
        OnDanceUnreacted.RemoveListener(player.MissedStep);
    }

    public void PlayerDanceReaction(PlayerManager player, DanceStep step) //cuando un player hace un paso esta es la reaccion
    {
        if(currentPuzzleStep != DanceStep.None){
            //bool anyPlayerIsCorrect = false; //esta variable no me queda clara, pareciera que no esta haciendo nada
            ReactToPlayersDance(player, step);
            VisualFeedbackToPlayerDance(/*anyPlayerIsCorrect*/  false);
        }
        currentDanceTriggered = true;
    }

    public abstract void ReactToPlayersDance(PlayerManager player,DanceStep step);
    
    public abstract void VisualFeedbackToPlayerDance(bool isPlayerDanceCorrect);

    public abstract void GeneralVisualFeedback(int counter);
    
    public virtual void PlayerEnter(PlayerManager player)
    {
        if(debug)Debug.Log("Player entered");
        if (player.IsAlreadyTargetPuzzle(this))
        {
            if (debug)Debug.Log("[RHYTHM_PUZZLE] Player already has this puzzle as it's active.");
            return;
        }
        player.AddTargetPuzzle(this);
        //playersInside.Add(player);
    }

    public virtual void PlayerLeave(PlayerManager player)
    {
        if(debug)Debug.Log("Player Leave");
        //playersInside.Remove(player);
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
        
        innerCounter++;
        if(debug)Debug.Log("innerCounter+1");
        //innerCounter = BeatManager.Instance.counter; no me parece que haya razon alguna para que esto se coordine con el global, la verdad. (Ssoar)
        
    }

    internal DanceStep GetStepToMatch(Timing currentBeatTiming) //dado el timing actual retorna el futureStep o el currentStep (NO ESPERA RECIBIR EL MISS TIMING)
    {
        if (currentBeatTiming == Timing.Early)
            return futurePuzzleStep;
        if (currentBeatTiming == Timing.Late)
            return currentPuzzleStep;
        Debug.LogError("[RhythmPuzzle] GetStepToMatch recibio un timing inesperado");
        return DanceStep.None;
    }
    
    internal bool IsTimingValid() // de acuerdo a currentBeatTiming, true si es Early o Late, false si es Miss
    {
        if (currentBeatTiming == Timing.Miss)
            return false;
        else
            return true;
    }
}
