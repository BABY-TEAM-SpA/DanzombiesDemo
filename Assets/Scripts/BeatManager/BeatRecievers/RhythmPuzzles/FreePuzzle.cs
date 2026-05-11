using System;
using System.Collections.Generic;
using UnityEngine;

public class FreePuzzle : RhythmPuzzle
{
    public override void PreparePuzzle()
    {
        activeDanceSequence = new SequenceStep();
    }

    public override void ReactToPlayersDance(PlayerManager player, DanceStep step)
    {
        //CONTINUE FROM HERE
    }
    
    public override void VisualFeedbackToPlayerDance(bool isPlayerDanceCorrect)
    {
    }

    public override void GeneralVisualFeedback(int counter)
    {
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
}