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
        videoPlayer.audioOutputMode = hasAudio?VideoAudioOutputMode.Direct:VideoAudioOutputMode.None;
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
    }
    void OnVideoEnd(VideoPlayer vp)
    {
        SceneChangeController.Instance.LoadScenes(levelToLoad);
    }

    public void ForceVideoEnd()
    {
        videoPlayer.Stop();
        OnVideoEnd(videoPlayer);
    }
}
