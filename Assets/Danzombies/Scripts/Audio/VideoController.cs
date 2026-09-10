using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [SerializeField] private SceneChangeController.LoadScenePack levelToLoad;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private bool hasAudio;
    [SerializeField] private bool shouldPlayOnStart;

    [Header("FMOD Configuration")]
    [SerializeField] private FMODUnity.EventReference fmodEvent; 
    [SerializeField] private AudioSource unityAudioSource;

    private FMOD.Studio.EventInstance videoAudioInstance;
    private FMOD.Studio.Bus masterBus; // Almacena el bus maestro para pausar/silenciar el resto

    void Start()
    {
        // Obtenemos el bus maestro de FMOD para controlar todo el audio previo
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        if (shouldPlayOnStart) PlayVideo();
    }

    private void OnEnable()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    void PlayVideo()
    {
        videoPlayer.clip = videoClip;
        
        if (hasAudio && !fmodEvent.IsNull && unityAudioSource != null)
        {
            // 1. Detener o mutear el audio previo antes de arrancar el nuevo
            // Opción A: Pausar absolutamente todo el juego (Recomendado para cinemáticas)
            masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 

            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, unityAudioSource);
            
            videoAudioInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(videoAudioInstance, transform);
            
            videoAudioInstance.start();
        }
        else
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        }
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StopAndReleaseAudio();
        SceneChangeController.Instance.LoadScenes(levelToLoad);
    }

    public void ForceVideoEnd()
    {
        videoPlayer.Stop();
        OnVideoEnd(videoPlayer);
    }

    private void StopAndReleaseAudio()
    {
        if (videoAudioInstance.isValid())
        {
            videoAudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            videoAudioInstance.release();
        }
    }

    private void OnDestroy()
    {
        StopAndReleaseAudio();
    }
}
