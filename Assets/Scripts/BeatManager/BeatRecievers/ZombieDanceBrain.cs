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
            
            if(view == "R") danceAnimator.SetBool("RightTrigger", true);
            else danceAnimator.SetBool("LeftTrigger", true);
        }
    }
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            danceAnimator.SetTrigger("Dance"+orientation);
        }
    }
    private void OnReleaseStepAction(DanceStep step)
    {
        
        danceAnimator.SetBool("RightTrigger", false);
        danceAnimator.SetBool("LeftTrigger", false);
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            danceAnimator.ResetTrigger("Dance"+orientation);
        }
    }

    private void MoveToPoint(Vector3 point)
    {
        Vector3 dif = new Vector3(this.transform.localPosition.x-point.x,this.transform.localPosition.y-point.y,0f);
        dif = dif.normalized;
        playerMovCtrl.Move(dif);
    }
}
