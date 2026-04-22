using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum DanceStep
{
    None,
    L_North,
    R_North,
    L_South,
    R_South,
    L_West,
    R_West,
    L_East,
    R_East
}


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
    
    public delegate void OnMusicEvent(DanceStep danceStep);
    public event OnMusicEvent OnPrepareStep;
    public event OnMusicEvent OnDanceStep;
    public delegate void OnMusicEvent2(DanceStep danceStep, DanceStep futureStep);
    public event OnMusicEvent2 OnReleaseStep;
    public UnityEvent OnPuzzleGetsActivateEvent = new UnityEvent();
    
    /*[Header("Players")]
    protected List<PlayerManager> playersInside = new List<PlayerManager>(); //se vuelve innecesario por que ahora reaccionan a eventos de estos players.*/
    
    [Header("FeedBack References")]
    [SerializeField] protected SpriteRenderer feedBack;

    public bool onBeatWindow {private set; get;} //cuando TRUE el beat es aceptado cuando FALSE es rechazado. Para checks por parte de PlayerManager.
    //si queremos trackear grados de acierto (GREAT, GOOD, MISS) se necesitaria otra variable similar
    //lo mismo para (EARLY, LATE) y cosas asi
    
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

        onBeatWindow = true;
    }

    public override void BeatAction(int counter)
    {
        if (!isActive) return;
        if(debug)Debug.Log("______Puzzle make "+currentPuzzleStep.ToString()+" at "+counter+" on "+AudioSettings.dspTime.ToString());
        OnDanceStep?.Invoke(currentPuzzleStep);
        GeneralVisualFeedback(innerCounter);
        
    }
    public override void PostBeatAction(int counter)
    {
        if (!isActive) return;
        //Debug.Log("PostBeat");
        //OnRhythmPuzzleBeatReaction(); supersedido por onBeatWindow y chequeo por parte del PlayerManager
        futurePuzzleStep = GetNextDanceStep();
        OnReleaseStep?.Invoke(currentPuzzleStep,futurePuzzleStep);
        onBeatWindow = false;
    }

    public void SubscribeToPlayerReactions(PlayerManager player) //llamado por PlayerManager cuando el puzzle se registra como activo
    {
        Debug.Log(player.OnDanceEvent);
        player.OnDanceEvent.AddListener(PlayerDanceReaction);
    }
    
    public void UnsubscribeToPlayerReactions(PlayerManager player) //llamado por PlayerManager cuando el puzzle se desregistra como activo
    {
        player.OnDanceEvent.RemoveListener(PlayerDanceReaction);
    }

    public void PlayerDanceReaction(PlayerManager player, DanceStep step) //cuando un player hace un paso esta es la reaccion
    {
        if(currentPuzzleStep != DanceStep.None){
            //bool anyPlayerIsCorrect = false; //esta variable no me queda clara, pareciera que no esta haciendo nada
            ReactToPlayersDance(player, currentPuzzleStep); //TODO: asegurar que solo se reaccione al baile correcto una vez
            VisualFeedbackToPlayerDance(/*anyPlayerIsCorrect*/  false);
        }
    }

    public abstract void ReactToPlayersDance(PlayerManager player,DanceStep step);
    
    public abstract void VisualFeedbackToPlayerDance(bool isPlayerDanceCorrect);

    public abstract void GeneralVisualFeedback(int counter);

    public abstract void PlayerHasNoFlow(PlayerManager player);
    
    public virtual void PlayerEnter(PlayerManager player)
    {
        if(debug)Debug.Log("Player entered");
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
        
        innerCounter = BeatManager.Instance.counter;
        
    }
}
