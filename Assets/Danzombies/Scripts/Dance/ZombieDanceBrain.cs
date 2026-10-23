using System;
using UnityEngine;

public class ZombieDanceBrain : DanceBrain
{
    //#region [METHODS]
    //public void Move(Vector2 direction, Action onFinished = null) => movCtrl.BeginScriptedMovememnt(direction, onFinished);

    //private void MoveToPoint(Vector3 point, float time)
    //{
    //    Vector2 dif = new Vector2(
    //        point.x - transform.localPosition.x,
    //        point.y - transform.localPosition.y
    //    );
    //    dif = dif.normalized;
    //    movCtrl.BeginScriptedMovememnt(dif);
    //}
    //#endregion

    #region [EVENTS]
    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType,int beatTimeMs, DanceStep danceStep)
    {
        if (isBussy)
        {
            Debug.Log($"{name} was bussy");
            return;
        }
        onDance?.Invoke(beatType,danceStep);
        danceAnimCtrl?.OnDanceBegin(danceStep,beatType);
    }
    #endregion
}
