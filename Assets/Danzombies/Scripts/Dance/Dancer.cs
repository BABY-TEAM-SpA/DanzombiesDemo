using System;
using UnityEngine;
using UnityEngine.Events;



public class DanceEventBuffer
{
    private delegate void OnPuzzle(RhythmPuzzle puzzle);
    private event OnPuzzle OnEnable;
    private event OnPuzzle OnReseat;
    private event OnPuzzle OnDisable;
    private delegate void OnDanceSetEvent(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs, DanceStep danceStep);
    
    private event OnDanceSetEvent OnDanceStep;
    
    private delegate void OnPreDanceSetEvent(int beatCounter, BeatManager.BeatType beatType, DanceStep danceStep);
    private event OnPreDanceSetEvent OnNextStep;
    
    public void AddListener(Dancer dancer)
    {
        OnEnable  += dancer.OnEnablePuzzle;
        OnReseat  += dancer.OnReseatPuzzle;
        OnDisable  += dancer.OnDisablePuzzle;
        OnDanceStep += dancer.OnDanceStepAction;
        OnNextStep += dancer.OnSetNextSetAction;
    }
    public void RemoveListener(Dancer dancer)
    {
        OnEnable  -= dancer.OnEnablePuzzle;
        OnReseat  -= dancer.OnReseatPuzzle;
        OnDisable  -= dancer.OnDisablePuzzle;
        OnDanceStep -= dancer.OnDanceStepAction;
        OnNextStep -= dancer.OnSetNextSetAction;
    }
    public void RemoveAllListeners()
    {
        //OnPrepareStep = null;
        //OnPreDanceStep = null;
        OnDanceStep = null;
        //OnReleaseStep = null;
        OnNextStep = null;
    }
    
    public void InvokeEnablePuzzle(RhythmPuzzle puzzle) => OnEnable?.Invoke(puzzle);
    public void InvokeReseatPuzzle(RhythmPuzzle puzzle) => OnReseat?.Invoke(puzzle);
    public void InvokeDisablePuzzle(RhythmPuzzle puzzle) => OnDisable?.Invoke(puzzle);
    
    public void InvokePreDance(int beat, BeatManager.BeatType beatType) 
    {
        //OnPreDanceStep?.Invoke(beat, beatType, DanceStep.None);
    }
    
    public void InvokeDance(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs, DanceStep danceStep)
    {
        OnDanceStep?.Invoke(beatCounter, beatType, beatTimeMs ,danceStep);
    }
    
    public void InvokeNextStep(int nextBeat, BeatManager.BeatType beatType, DanceStep nextDanceStep) =>OnNextStep?.Invoke(nextBeat, beatType, nextDanceStep); 
}


public class Dancer: MonoBehaviour
{
    protected DanceStep currentDanceStep;
    public UnityEvent onActivated;
    public UnityEvent<BeatManager.BeatType,DanceStep> onDance;
    public UnityEvent onDeactivated;
    public Action OnCompleted;

    public virtual void OnEnablePuzzle(RhythmPuzzle puzzl)
    {
        onActivated?.Invoke();
    }
    public virtual void OnReseatPuzzle(RhythmPuzzle puzzl)
    {
        currentDanceStep = DanceStep.None;
    }

    public virtual void OnPreDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        //Debug.Log("OnPreDanceStepAction Dancer:"+this.transform.name);
    }
    
    public virtual void OnDanceStepAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs, DanceStep danceStep)
    {
        currentDanceStep = danceStep;
        onDance?.Invoke(beatType,danceStep);
    }
    
    public virtual void OnSetNextSetAction(int nextBeat, BeatManager.BeatType beatType,DanceStep nextDanceStep) { }

    public virtual void OnDisablePuzzle(RhythmPuzzle puzzl)
    {
        onDeactivated?.Invoke();
    }
    public virtual void React(ExpressionType exp) { }
}