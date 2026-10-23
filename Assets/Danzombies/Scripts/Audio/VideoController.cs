using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [SerializeField] private LoadScenePack levelToLoad;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private bool hasAudio;
    [SerializeField] private bool shouldPlayOnStart;

    [Header("FMOD Configuration")]
    [SerializeField] private FMODUnity.EventReference fmodEvent;

    private FMOD.Studio.EventInstance videoAudioInstance;
    private FMOD.Studio.Bus masterBus; // Almacena el bus maestro para pausar/silenciar el resto

    void Start()
    {
        videoPlayer.clip = videoClip;
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        videoPlayer.started += OnVideoStarted;
        videoPlayer.Prepare();
        
    }

    private void OnEnable()
    {
        videoPlayer.prepareCompleted += OnPrepareCompleted;
        videoPlayer.started += OnVideoStarted;
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDisable()
    {
        videoPlayer.prepareCompleted -= OnPrepareCompleted;
        videoPlayer.started -= OnVideoStarted;
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
    void OnPrepareCompleted(VideoPlayer vp)
    {
        Debug.Log("Preparation done.");
        if (shouldPlayOnStart) PlayVideo();
    }

    void OnVideoStarted(VideoPlayer vp)
    {
        //Debug.Log("Video has started.");
        if (hasAudio && !fmodEvent.IsNull)
        {
            videoAudioInstance.start();
        }
        else Debug.Log("OnVideoStarted tried to play an AudioSource that doesn't exist.");
    }

    void PlayVideo()
    {
        
        if (hasAudio && !fmodEvent.IsNull)
        {
            masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 
            videoAudioInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(videoAudioInstance, transform);
        }
        //videoPlayer.waitForFirstFrame = true;
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
