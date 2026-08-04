using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieDanceZone : RhythmPuzzle
{
    #region [VARIABLES]
    [Header("Zombies Dance Settings")]
    [SerializeField] private List<ZombieDanceBrain> zombies = new List<ZombieDanceBrain>();

    public SequenceStep danceSequence = new SequenceStep();
    #endregion

    #region [UNITY]
    private void OnDisable()
    {
        foreach (ZombieDanceBrain zombie in zombies)
            currentDanceData.listeners.RemoveListener(zombie);
    }

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
    #region RhythmPuzzle - Setup
    public override void PreparePuzzle()
    {
        SetSequence(danceSequence);
        foreach (ZombieDanceBrain zombie in zombies)
            currentDanceData.listeners.AddListener(zombie);
    }

    public override void ActivatePuzzle(bool activate)
    {
        base.ActivatePuzzle(activate);
        foreach (ZombieDanceBrain zombie in zombies)
            zombie.ActivateEntity(activate);
    }
    #endregion

    #region RhythmPuzzle - Update
    public override void OnDanceSequenceCleared() { }
    #endregion

    #region RhythmPuzzle - Player
    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp)
    {
        if (exp == currentReaction)
            return;

        currentReaction = exp;
        foreach (ZombieDanceBrain zombie in zombies)
            zombie.React(currentReaction);
    }

    public override void PlayerGetDamaged()
    {
        PlayerLeave(playersInside);
    }
    #endregion

    #region BeatReceiver
    public override void PreBeatAction(int counter)
    {
        if (debug)
            Debug.Log("___Puzzle PreBeat on " + AudioManager.Instance.SongPositionSeconds().ToString());

        PlayerHasDanced = false;
        currentDanceData.SetDanceStep(counter);
        currentDanceData.listeners.InvokePrepare(currentDanceData.DanceStep, currentDanceData.NextDanceStep);
    }

    public override void BeatAction(int counter)
    {
        if (debug)
            Debug.Log("_____Puzzle Beat make " + currentDanceData.DanceStep.ToString() + " at " + counter + " on " + AudioManager.Instance.SongPositionSeconds().ToString());
        
        currentDanceData.listeners.InvokeDance(currentDanceData.DanceStep, currentDanceData.NextDanceStep);
    }

    public override void PostBeatAction(int counter)
    {

        if (debug)
            Debug.Log("___Puzzle PostBeat on " + AudioManager.Instance.SongPositionSeconds().ToString());

        CheckPlayerPost();
        currentDanceData?.listeners.InvokeRealease(currentDanceData.DanceStep, currentDanceData.NextDanceStep);
        currentDanceData?.SetFutureDanceStep(counter);
        currentDanceData.DanceStep = DanceStep.None;
        PlayerHasDanced = false;
    }
    #endregion

    #region Helpers
    public void RefreshZombies()
        => zombies = GetComponentsInChildren<ZombieDanceBrain>().ToList();
    #endregion
    #endregion
}