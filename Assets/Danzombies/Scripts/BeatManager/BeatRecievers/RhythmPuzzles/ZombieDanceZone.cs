using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDanceZone : RhythmPuzzle
{
    [Header("Zombies Dance Settings")]
    [SerializeField] private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();
    public SequenceStep danceSequence =new SequenceStep();
    public override void PreparePuzzle()
    {
        SetSequence(danceSequence);
        foreach (ZombieDanceBrain zombie in zombies)
        {
            currentDanceData.listeners.AddListener(zombie);
        }
    }
    
    public override void ActivatePuzzle(bool activate)
    {
        base.ActivatePuzzle(activate);
        foreach (ZombieDanceBrain zombie in zombies)
        {
            zombie.ActivateEntity(activate);
        }
    }

    public override void PreBeatAction(int counter)
    {
        if(debug) Debug.Log("___Puzzle PreBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        PlayerHasDanced = false;
        currentDanceData.SetDanceStep(counter);
        currentDanceData.listeners.InvokePrepare(currentDanceData.DanceStep, currentDanceData.NextDanceStep);
    }

    public override void BeatAction(int counter)
    {
        if(debug) Debug.Log("_____Puzzle Beat make "+currentDanceData.DanceStep.ToString()+" at "+counter+" on "+AudioManager.Instance.SongPositionSeconds().ToString());
        currentDanceData.listeners.InvokeDance(currentDanceData.DanceStep, currentDanceData.NextDanceStep);
    }

    public override void PostBeatAction(int counter)
    {
        
        if(debug) Debug.Log("___Puzzle PostBeat on "+AudioManager.Instance.SongPositionSeconds().ToString());
        CheckPlayerPost();
        currentDanceData?.listeners.InvokeRealease(currentDanceData.DanceStep, currentDanceData.NextDanceStep);
        currentDanceData?.SetFutureDanceStep(counter);
        currentDanceData.DanceStep = DanceStep.None;
        PlayerHasDanced = false;
    }

    private void OnDisable()
    {
        foreach (ZombieDanceBrain zombie in zombies)currentDanceData.listeners.RemoveListener(zombie);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            PlayerEnter(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            PlayerLeave(player);
        }
    }
   
    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp)
    {
        if (exp == currentReaction) return;
        currentReaction = exp;
        foreach (ZombieDanceBrain zombie in zombies)
        {
            zombie.React(currentReaction);
        }
    }

    public override void PlayerGetDamaged()
    {
        PlayerLeave(playersInside);
    }

    public override void OnDanceSequenceCleared()
    {
        throw new NotImplementedException();
    }
}