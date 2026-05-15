using System;
using UnityEngine;

public class ZombieDanceBrain : DanceBrain
{
    public void Connect(RhythmPuzzle puzzle)
    {
        puzzle.OnDanceStep += OnDanceStepAction;
    }
    public void Disconnect(RhythmPuzzle puzzle)
    {
        puzzle.OnDanceStep -= OnDanceStepAction;
    }

    
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
        {
            danceAnimCtrl?.OnDanceBegin(step);
        }
    }
    

    private void MoveToPoint(Vector3 point, float time)
    {
        Vector3 dif = new Vector3(this.transform.localPosition.x-point.x,this.transform.localPosition.y-point.y,0f);
        dif = dif.normalized;
        movCtrl.SetDirectionToMove(dif);
        EnableMovement(true);
        Invoke("EnableMovement", time);
    }

    public override void OnDance(DanceStep step)
    {
        //throw new NotImplementedException();
    }
}
