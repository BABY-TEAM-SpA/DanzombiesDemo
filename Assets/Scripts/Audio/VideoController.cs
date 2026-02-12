using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [SerializeField] private string levelToLoad;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private bool shouldStopMusic;
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
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
        if (shouldStopMusic) AudioManager.Instance.StopSong();
    }
    void OnVideoEnd(VideoPlayer vp)
    {
        SceneChangeController.Instance.LoadScene(levelToLoad);
    }
}
