using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
class EffectPair
{
    public ComboSO combo;
    public UnityEvent OnComboExecuted;
}

public class FreePuzzle : RhythmPuzzle
{
    [SerializeField] EffectPair[] activeCombos;
    PlayerDanceMemory storedDanceMemory;
    PlayerManager storedPlayer;

    public override void PreparePuzzle()
    {
        activeDanceSequence = new SequenceStep();
    }

    public override void ReactToPlayersDance(PlayerManager player, DanceStep step)
    {
        PlayerDanceMemory danceMemory = player.GetDanceMemory();
        if (!IsTimingValid() || currentDanceTriggered)
            danceMemory.InitializeMemory();
        else
        {
            foreach(EffectPair activeCombo in activeCombos)
            {
                if (danceMemory.MatchesMemory(activeCombo.combo.sequence))
                    activeCombo.OnComboExecuted?.Invoke();
            }
        }
    } 
    
    public override void VisualFeedbackToPlayerDance(bool isPlayerDanceCorrect)
    {
    }

    public override void GeneralVisualFeedback(int counter)
    {
    }

    public override void PostBeatAction(int counter)
    {
        base.PostBeatAction(counter);
        if (!currentDanceTriggered)
            storedDanceMemory?.RememberStep(storedPlayer, DanceStep.None);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            PlayerEnter(player);
            storedDanceMemory = player.GetDanceMemory();
            storedDanceMemory.SetMemorySize(GetLargestComboLength());
            storedPlayer = player;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            PlayerLeave(player);
            player.GetDanceMemory().SetMemorySize(0);
            storedPlayer = null;
            storedDanceMemory = null;
        }
    }

    private int GetLargestComboLength()
    {
        int longest = 0;
        foreach(EffectPair activeCombo in activeCombos)
        {
            if (activeCombo.combo.sequence.Count > longest)
                longest = activeCombo.combo.sequence.Count;
        }
        return longest;
    }

    public void DebugPrint(string message){
        Debug.Log(message);
    }
}