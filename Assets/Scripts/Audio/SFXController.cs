using UnityEngine;
using System;


public class SFXController : MonoBehaviour
{
    public void OnWalkingStep()
    {
        if(null!=AudioManager.Instance)PlaySFX(AudioManager.Instance.playerStepSFX);
    }

    public void OnClappingEvent()
    {
         if(null!=AudioManager.Instance)PlaySFX(AudioManager.Instance.playerClapSFX);
    }

    public void OnHardMove()
    {
        //To Implement
        //if(null!=AudioManager.Instance)
    }

    private void PlaySFX(AudioClip clip)
    {
        
        AudioSource oneShot = Instantiate<AudioSource>(AudioManager.Instance.SFXplayer);
        oneShot.volume = AudioManager.Instance.SFXsettings.Volume;
        oneShot.pitch = UnityEngine.Random.Range(AudioManager.Instance.SFXsettings.pitchMin, AudioManager.Instance.SFXsettings.pitchMax);
        oneShot.clip = clip;
        oneShot.Play();
        Destroy(oneShot.gameObject, clip.length / Math.Abs(oneShot.pitch));
    }
}
