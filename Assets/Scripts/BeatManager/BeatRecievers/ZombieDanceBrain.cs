using System;
using UnityEngine;

public class ZombieDanceBrain : MonoBehaviour
{
    [SerializeField] private PlayerMovementController playerMovCtrl;
    [SerializeField] private PlayerAnimatorController playerAnimatorController;
    [SerializeField] private Animator danceAnimator;
    
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
            if(view == "R") danceAnimator.SetBool("RightLook", true);
            else danceAnimator.SetBool("RightLook", false);
        }
    }
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            danceAnimator.SetBool("DanceStep"+orientation[0],true);
            danceAnimator.SetTrigger("Dance");
        }
    }
    private void OnReleaseStepAction(DanceStep step)
    {
        danceAnimator.SetBool("RightLook", false);
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            danceAnimator.SetBool("DanceStep" + orientation[0], false);
            danceAnimator.ResetTrigger("Dance");
        }
    }

    private void MoveToPoint(Vector3 point)
    {
        Vector3 dif = new Vector3(this.transform.localPosition.x-point.x,this.transform.localPosition.y-point.y,0f);
        dif = dif.normalized;
        playerMovCtrl.Move(dif);
    }
}
