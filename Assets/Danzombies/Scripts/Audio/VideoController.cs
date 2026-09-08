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
    // Usamos el tipo correcto y moderno de FMOD
    [SerializeField] private FMODUnity.EventReference fmodEvent; 
    [SerializeField] private AudioSource unityAudioSource;

    private FMOD.Studio.EventInstance videoAudioInstance;

    void Start()
    {
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

        // Comprobamos si el evento de FMOD es válido antes de usarlo
        if (hasAudio && !fmodEvent.IsNull && unityAudioSource != null)
        {
            // 1. Enrutar el audio del video hacia el AudioSource de Unity
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, unityAudioSource);

            // 2. Crear la instancia usando la EventReference
            videoAudioInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
            
            // 3. Vincular la posición 3D (opcional pero recomendado si el video está en el mundo)
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(videoAudioInstance, transform);
            
            videoAudioInstance.start();
        }
        else
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        }

        // 4. Iniciar el video en sincronía
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
