using UnityEngine;

public class ZombieDanceBrain : DanceBrain
{
    #region [METHODS
    public void Connect(RhythmPuzzle puzzle) => puzzle.OnDanceStep += OnDanceStepAction;
    public void Disconnect(RhythmPuzzle puzzle) => puzzle.OnDanceStep -= OnDanceStepAction;

    public override void OnDance(DanceStep step)
    {
        //throw new NotImplementedException();
    }

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
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
            danceAnimCtrl?.OnDanceBegin(step);
    }
    #endregion
}
