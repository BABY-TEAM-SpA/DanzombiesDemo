using System;
using UnityEngine;
using UnityEngine.Events;






public abstract class RhythmPuzzle : BeatReciever
{

    [Header("Rhythm Puzzle Settings")]
    [SerializeField] protected bool debug;
    [SerializeField] bool activateOnStart;
    protected DanceEventManager eventManager = new DanceEventManager();
    [SerializeField] protected DanceSequence danceSequence;
    protected DanceStep currentStep;
    
    protected void Start()
    {
        PreparePuzzle();
        if (activateOnStart) SetActivePuzzle(true);
    }
    private void OnDisable()
    {
        eventManager.RemoveAllListeners();
    }
    
    
    public abstract void PreparePuzzle();
    
    public virtual void SetActivePuzzle(bool activate)
    {
        isActive = activate;
        
        if(isActive)
        {
            if(debug)Debug.Log($"Starting PUZZLE ({name})");
            eventManager.InvokeEnablePuzzle(this);
        } else
        {
            if(debug)Debug.Log($"Stopping PUZZLE ({name})");
            eventManager.InvokeDisablePuzzle(this);
        }
        if (!activate) return;
        
    }
    

    protected void SetSequence(DanceSequence sequence)
    {
        danceSequence = sequence;
    }

    public virtual void OnPuzzleCompleted()
    {
        //Debug.Log("Puzzle Is Over");
        SetActivePuzzle(false);
    }

}
