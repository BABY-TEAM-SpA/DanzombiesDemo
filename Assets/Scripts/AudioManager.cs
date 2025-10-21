using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _MusicSource;
    [SerializeField] private AudioSource _SFXSource;
    
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
   
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
            //DontDestroyOnLoad(this.gameObject);
        }
        
    }

    public void PlayMusic(AudioClip clip)
    {
        _MusicSource.clip = clip;
        _MusicSource.Play();
    }


    public void PlaySFX(AudioClip clip, bool randomPitch = true)
    {
        if (randomPitch)
        {
            AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            //audioSource.volume = GeneralVolume;
            audioSource.clip = clip;
            audioSource.Play();
            Destroy(audioSource, clip.length / Math.Abs(audioSource.pitch));
        }
        else
        {
            _SFXSource.PlayOneShot(clip);
        }
    }
    
}
