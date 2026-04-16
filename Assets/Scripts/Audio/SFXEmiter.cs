using System;
using UnityEngine;

public class SFXEmiter : MonoBehaviour
{
    public void PlaySFX(AudioClip clip)
    {
        AudioSource oneShot = Instantiate<AudioSource>(AudioManager.Instance.SFXplayer);
        oneShot.volume = AudioManager.Instance.SFXsettings.Volume;
        oneShot.pitch = UnityEngine.Random.Range(AudioManager.Instance.SFXsettings.pitchMin, AudioManager.Instance.SFXsettings.pitchMax);
        oneShot.clip = clip;
        oneShot.Play();
        Destroy(oneShot.gameObject, clip.length / Math.Abs(oneShot.pitch));
    }
}
