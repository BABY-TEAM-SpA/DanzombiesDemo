using UnityEngine;
using UnityEngine.Events;



public class DanceEventManager
{
    private delegate void OnPuzzle(RhythmPuzzle puzzle);
    private event OnPuzzle OnEnable;
    private event OnPuzzle OnReseat;
    private event OnPuzzle OnDisable;
    private delegate void OnDanceSetEvent(int beat, BeatManager.BeatType beatType,DanceStep danceStep);
    private event OnDanceSetEvent OnPrepareStep;
    private event OnDanceSetEvent OnDanceStep;
    private event OnDanceSetEvent OnReleaseStep;
    private event OnDanceSetEvent OnNextStep;
    
    public void AddListener(Dancer dancer)
    {
        OnEnable  += dancer.OnEnablePuzzle;
        OnReseat  += dancer.OnReseatPuzzle;
        OnDisable  += dancer.OnDisablePuzzle;
        OnPrepareStep += dancer.OnPrepareStepAction;
        OnDanceStep += dancer.OnDanceStepAction;
        OnReleaseStep += dancer.OnReleaseStepAction;
        OnNextStep += dancer.OnSetNextSetAction;
    }
    public void RemoveListener(Dancer dancer)
    {
        OnEnable  -= dancer.OnEnablePuzzle;
        OnReseat  -= dancer.OnReseatPuzzle;
        OnDisable  -= dancer.OnDisablePuzzle;
        OnPrepareStep -= dancer.OnPrepareStepAction;
        OnDanceStep -= dancer.OnDanceStepAction;
        OnReleaseStep -= dancer.OnReleaseStepAction;
        OnNextStep -= dancer.OnSetNextSetAction;
    }
    public void RemoveAllListeners()
    {
        OnPrepareStep = null;
        OnDanceStep = null;
        OnReleaseStep = null;
        OnNextStep = null;
    }
    
    public void InvokeEnablePuzzle(RhythmPuzzle puzzle) => OnEnable?.Invoke(puzzle);
    public void InvokeReseatPuzzle(RhythmPuzzle puzzle) => OnReseat?.Invoke(puzzle);
    public void InvokeDisablePuzzle(RhythmPuzzle puzzle) => OnDisable?.Invoke(puzzle);

    public void InvokePrepare(int beat, BeatManager.BeatType beatType, DanceStep danceStep)
    {
        if(danceStep!=DanceStep.None)OnPrepareStep?.Invoke(beat, beatType, danceStep);
    }
    public void InvokeDance(int beat, BeatManager.BeatType beatType,DanceStep danceStep) 
    {
        if(danceStep!=DanceStep.None)OnDanceStep?.Invoke(beat, beatType, danceStep);
    }
    public void InvokeRealease(int beat, BeatManager.BeatType beatType,DanceStep danceStep)
    {
        if(danceStep!=DanceStep.None)OnReleaseStep?.Invoke(beat, beatType, danceStep);
    }
    public void InvokeNextStep(int nextBeat, BeatManager.BeatType beatType, DanceStep nextDanceStep) =>OnNextStep?.Invoke(nextBeat, beatType, nextDanceStep); 
}


public class Dancer: MonoBehaviour
{
    protected DanceStep currentDanceStep;
    protected int currentBeat;
    protected BeatManager.BeatType currentBeatType;
    public UnityEvent<DanceStep> onDance;
    public UnityEvent<ExpressionType> onReaction;

    public virtual void OnEnablePuzzle(RhythmPuzzle puzzl){}
    public virtual void OnReseatPuzzle(RhythmPuzzle puzzl)
    {
        currentDanceStep = DanceStep.None;
        currentBeat = 0;
    }
    public virtual void OnDisablePuzzle(RhythmPuzzle puzzl){}
    
    public virtual void OnPrepareStepAction(int beat, BeatManager.BeatType beatType,DanceStep danceStep)
    {
        currentDanceStep = danceStep;
    }

    public virtual void OnDanceStepAction(int beat, BeatManager.BeatType beatType,DanceStep danceStep){}

    public virtual void OnReleaseStepAction(int beat, BeatManager.BeatType beatType,DanceStep danceStep)
    {
        currentDanceStep = DanceStep.None;
    }
    public virtual void OnSetNextSetAction(int nextBeat, BeatManager.BeatType beatType,DanceStep nextDanceStep) { }
    public virtual void React(ExpressionType exp) { }
}