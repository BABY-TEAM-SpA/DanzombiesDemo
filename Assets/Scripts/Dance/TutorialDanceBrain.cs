using UnityEngine;

public class TutorialDanceBrain : MonoBehaviour
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
            //if(view == "R") playerAnimCtrl?.animator.SetBool("RightDanceDir", true);
            //else playerAnimCtrl?.animator.SetBool("LeftDanceDir", true);
        }
    }
    private void OnDanceStepAction(DanceStep step)
    {
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            //playerAnimCtrl?.animator.SetBool("DanceStep"+orientation[0],true);
            //playerAnimCtrl?.animator.SetTrigger("Dance");
        }
    }
    private void OnReleaseStepAction(DanceStep step)
    {
        //playerAnimCtrl?.animator.SetBool("RightDanceDir", false);
        //playerAnimCtrl?.animator.SetBool("LeftDanceDir", false);
        if (step != DanceStep.None)
        {
            string orientation = step.ToString().Remove(0,2);
            //playerAnimCtrl?.animator.SetBool("DanceStep" + orientation[0], false);
            //playerAnimCtrl?.animator.ResetTrigger("Dance");
        }
    }
}
