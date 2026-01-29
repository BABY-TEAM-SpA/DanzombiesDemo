using System;
using UnityEngine;

[Serializable]
public class SoundSettings
{
    [Range(0f, 1f)]public float Volume;
    [Range(0.5f, 1f)]public float pitchMin;
    [Range(1f, 1.5f)]public float pitchMax;
}



public class AudioManager : MonoBehaviour
{
    public SoundSettings MusicSettings = new SoundSettings();
    public SoundSettings SFXsettings = new SoundSettings();

    public AudioSource SFXplayer;
    public AudioClip playerStepSFX;
    public AudioClip playerClapSFX;


    public static AudioManager Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
    }
}
