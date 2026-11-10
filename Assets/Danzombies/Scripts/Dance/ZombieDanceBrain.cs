using System;
using UnityEngine;

public class ZombieDanceBrain : DanceBrain
{
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
