using System;
using UnityEngine;

public class ZombieDanceBrain : DanceBrain
{
    public void Connect(RhythmPuzzle puzzle)
    {
        puzzle.OnPrepareStep += OnPrepareStepAction;
        puzzle.OnDanceStep += OnDanceStepAction;
        puzzle.OnReleaseStep += OnReleaseStepAction;
    }
    public void Disconnect(RhythmPuzzle puzzle)
    {
        puzzle.OnPrepareStep -= OnPrepareStepAction;
        puzzle.OnDanceStep -= OnDanceStepAction;
        puzzle.OnReleaseStep -= OnReleaseStepAction;
    }

    private void OnPrepareStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
        {
            string view = step.ToString()[0].ToString();
            if(view == "R") playerAnimCtrl?.animator.SetBool("RightDanceDir", true);
            else playerAnimCtrl?.animator.SetBool("LeftDanceDir", true);
        }
    }
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            playerAnimCtrl?.animator.SetBool("DanceStep"+orientation[0],true);
            //Debug.Log("____________Zombie Danced "+step.ToString()+" at "+ AudioSettings.dspTime.ToString());
            playerAnimCtrl?.animator.SetTrigger("Dance");
        }
    }
    private void OnReleaseStepAction(DanceStep step)
    {
        playerAnimCtrl?.animator.SetBool("RightDanceDir", false);
        playerAnimCtrl?.animator.SetBool("LeftDanceDir", false);
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            playerAnimCtrl?.animator.SetBool("DanceStep" + orientation[0], false);
            playerAnimCtrl?.animator.ResetTrigger("Dance");
        }
    }

    private void MoveToPoint(Vector3 point, float time)
    {
        Vector3 dif = new Vector3(this.transform.localPosition.x-point.x,this.transform.localPosition.y-point.y,0f);
        dif = dif.normalized;
        playerMovCtrl.SetDirectionToMove(dif);
        EnableMovement(true);
        Invoke("EnableMovement", time);
    }
}
