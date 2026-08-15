using UnityEngine;
using UnityEngine.Events;

public class DanceEventManager
{
    private delegate void OnDanceSetEvent(DanceStep danceStep);
    private event OnDanceSetEvent OnPrepareStep;
    private event OnDanceSetEvent OnDanceStep;
    private event OnDanceSetEvent OnReleaseStep;
    private event OnDanceSetEvent OnNextStep;
    
    public void AddListener(Dancer dancer)
    {
        OnPrepareStep += dancer.OnPrepareStepAction;
        OnDanceStep += dancer.OnDanceStepAction;
        OnReleaseStep += dancer.OnReleaseStepAction;
        OnNextStep += dancer.OnSetNextSetAction;
    }
    public void RemoveListener(Dancer dancer)
    {
        OnPrepareStep -= dancer.OnPrepareStepAction;
        OnDanceStep -= dancer.OnDanceStepAction;
        OnReleaseStep -= dancer.OnReleaseStepAction;
        OnNextStep += dancer.OnSetNextSetAction;
    }
    public void RemoveAllListeners()
    {
        OnPrepareStep = null;
        OnDanceStep = null;
        OnReleaseStep = null;
        OnNextStep = null;
    }

    public void InvokePrepare(DanceStep danceStep) => OnPrepareStep?.Invoke(danceStep);
    public void InvokeDance(DanceStep danceStep) => OnDanceStep?.Invoke(danceStep);
    public void InvokeRealease(DanceStep danceStep) => OnReleaseStep?.Invoke(danceStep);
    public void InvokeNextStep(DanceStep nextDanceStep) =>OnNextStep?.Invoke(nextDanceStep); 
}


public class Dancer: MonoBehaviour
{
    protected DanceStep currentDanceStep;
    protected bool danceWindow=false;
    public UnityEvent<DanceStep> onDance;
    public UnityEvent<ExpressionType> onReaction;

    public virtual void OnPrepareStepAction(DanceStep step)
    {
        danceWindow = true;
        currentDanceStep = step;
    }

    public virtual void OnDanceStepAction(DanceStep step){}

    public virtual void OnReleaseStepAction(DanceStep step)
    {
        danceWindow = false;
        currentDanceStep = DanceStep.None;
    }
    public virtual void OnSetNextSetAction(DanceStep step) { }
    public virtual void React(ExpressionType exp) { }
}