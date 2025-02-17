using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BeatManager;

public class DanceBrain : MonoBehaviour
{
    [SerializeField] private bool isPlayer = false;
    [SerializeField] Animator m_Animator;

    public List<string> danceStepLearned = new List<string>();

    private void OnEnable()
    {
        BeatManager.OnPlay += OnPlayEvent;
       
    }

    private void OnDisable()
    {
        BeatManager.OnPlay -= OnPlayEvent;
       
    }
    public void OnPlayEvent(float beatDuration)
    {
        m_Animator.SetFloat("Blend", 1/ beatDuration);
        m_Animator.Play(0);
    }
    public void LearnDance(string dance)
    {
        danceStepLearned.Add(dance);
    }

   private void DanceAnimationOverride(AnimatorOverrideController overrideController)
    {
        m_Animator.runtimeAnimatorController = overrideController;

    }

    public void MakeDance(string pose)
    {
        if (danceStepLearned.Contains(pose)){
            PoseAnimationData poseData = DanceAnimationManager.Instance.GetPoseAnimationData(pose);
            if (poseData != null)
            {
                DanceAnimationOverride(poseData.overrideController);
                m_Animator.SetTrigger("Dance");
            }
        }
        else
        {
            //Debug.Log("No danceStep Found");
        }
        
    }

}
