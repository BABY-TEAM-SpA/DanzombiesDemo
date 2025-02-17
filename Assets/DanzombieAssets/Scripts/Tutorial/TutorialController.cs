using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : BeatReciever
{

    [SerializeField] private ScriptableChoreography choreoData;
    private string actualCodePose;
    private int actualPose =0;
    private int succesfulSteps =0;
    [SerializeField] private int stepsToComplete = 3;

    public void PlayTutorial()
    {
        actualCodePose = choreoData.choreography[actualPose].GetDanceCode();
    }
    public void SuccesDance()
    {

    }

    public void CompleteTutorial()
    {
        actualPose += 1;

    }

    public override void OnPlaySongAction(float beatDuration)
    {

    }

    public override void PreBeatAction()
    {
         
    }

    public override void BeatAction()
    {
        /// implement 
    }

    public override void PostBeatAction()
    {
        /// implement 
    }
}
