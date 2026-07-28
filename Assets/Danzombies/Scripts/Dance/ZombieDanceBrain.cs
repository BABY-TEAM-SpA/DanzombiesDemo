using System;
using UnityEngine;

public class ZombieDanceBrain : DanceBrain
{
    #region [METHODS]
    
    private void OnDisable()
    {
        //throw new NotImplementedException();
    }

    public override void OnDance(DanceStep step)
    {
        //throw new NotImplementedException();
    }

    public void Move(Vector2 direction, float time, Action onFinished = null) => movCtrl.MoveForSeconds(direction, time, onFinished);
    private void MoveToPoint(Vector3 point, float time)
    {
        Vector2 dif = new Vector2(
            point.x - transform.localPosition.x,
            point.y - transform.localPosition.y
        );
        dif = dif.normalized;
        movCtrl.MoveForSeconds(dif, time);
    }    
    #endregion

    #region [EVENTS]
    
    public virtual void OnPrepareStepAction(DanceStep step,DanceStep nextStep){ }
    public virtual void OnDanceStepAction(DanceStep step,DanceStep nextStep)
    {
        if (step != DanceStep.None)
            danceAnimCtrl?.OnDanceBegin(step);
    }
    public virtual void OnReleaseStepAction(DanceStep step, DanceStep futureStep){ }
    #endregion
}
