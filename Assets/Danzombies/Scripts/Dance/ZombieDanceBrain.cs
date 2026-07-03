using System;
using System.Collections.Generic;
using System.Linq;
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
        Vector2 dif = new Vector2(this.transform.localPosition.x-point.x,this.transform.localPosition.y-point.y);
        dif = dif.normalized;
        movCtrl.MoveForSeconds(dif, time);
    }

    public override void OnDance(DanceStep step)
    {
        //throw new NotImplementedException();
    }
}
