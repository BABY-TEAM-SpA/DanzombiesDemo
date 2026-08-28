using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;






public abstract class RhythmPuzzle : BeatReciever
{

    [Header("Rhythm Puzzle Settings")]
    [SerializeField] protected bool debug;
    [SerializeField] bool activateOnStart;
    protected DanceEventManager eventManager = new DanceEventManager();
    protected DanceSequence currentDanceSequence;
    protected DanceStep currentStep;
    protected bool availableToDance = false;

    [SerializeField] protected PlayerInputEvent[] playerInputs;
    [Serializable]
    public class PlayerInputEvent
    {
        public BeatReciever.BeatFeedback feedback;
        public UnityEvent OnPlayerSuccess;
    }

    public void ResolvePlayerInput(BeatReciever.BeatFeedback fb)
    {
        PlayerInputEvent e = playerInputs.FirstOrDefault(p => p.feedback == fb);
        e?.OnPlayerSuccess?.Invoke();
    }
    
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
        availableToDance = false;
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
        currentDanceSequence = sequence;
    }

    public virtual void OnPuzzleCompleted()
    {
        //Debug.Log("Puzzle Is Over");
        SetActivePuzzle(false);
    }

}
