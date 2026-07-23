using System;
using System.Linq;
using UnityEngine;

public class WandererPuzzle : RhythmPuzzle
{
    [SerializeField] private ZombieDanceBrain zombie;
    public SequenceStep danceSequence;
    public bool RandomizeSteps= false;

    public override void PreparePuzzle()
    {
        SetSequence(danceSequence);
        //zombie.Connect(this);
    }

    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp)
    {
        throw new System.NotImplementedException();
    }
    

    public override void OnDanceSequenceCleared()
    {
        throw new NotImplementedException();
    }


    public override void OnUpdateSongAction()
    {
        //throw new System.NotImplementedException();
    }

    public override void PreBeatAction(int counter)
    {
        throw new NotImplementedException();
    }

    public override void BeatAction(int counter)
    {
        throw new NotImplementedException();
    }

    public override void PostBeatAction(int counter)
    {
        throw new NotImplementedException();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player))
        {
            // if (zoneMaterial != null) zoneMaterial.SetFloat("_ActiveState", 0f);  
            PlayerLeave(player);
        }
    }
    protected override void PlayerEnter(PlayerManager player)
    {
        base.PlayerEnter(player);
        LevelUIController.Instance?.UpdateZombieFeedbackUI(true);
        
    }
}
