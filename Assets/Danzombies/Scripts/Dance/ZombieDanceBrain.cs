using System;
using UnityEngine;

public class ZombieDanceBrain : DanceBrain
{
    #region [METHODS]
    

    public void Move(Vector2 direction, float time, Action onFinished = null) => movCtrl.BeginScriptedMovememnt(time, direction, onFinished);
    private void MoveToPoint(Vector3 point, float time)
    {
        Vector2 dif = new Vector2(
            point.x - transform.localPosition.x,
            point.y - transform.localPosition.y
        );
        dif = dif.normalized;
        movCtrl.BeginScriptedMovememnt(time, dif);
    }    
    #endregion

    #region [EVENTS]
    
    public override void OnDanceStepAction(int beat,BeatManager.BeatType beatType, DanceStep step)
    {
        onDance?.Invoke(step);
        danceAnimCtrl?.OnDanceBegin(step);
    }
    #endregion
}
