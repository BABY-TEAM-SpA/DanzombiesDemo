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
        if(debug)Debug.Log($"STARTING PUZZLE ({name})");
        PreparePuzzle();
        if (activateOnStart)
            SetActivePuzzle(true);
    }
    
    
    public abstract void PreparePuzzle();
    
    public virtual void SetActivePuzzle(bool activate)
    {
        isActive = activate;
        if (!activate)
            return;
    }
    

    protected void SetSequence(DanceSequence sequence)
    {
        danceSequence = sequence;
    }

    public abstract void OnPuzzleCompleted();

}
