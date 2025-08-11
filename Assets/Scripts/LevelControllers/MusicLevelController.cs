using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicLevelController : MonoBehaviour
{
    [SerializeField] private bool shouldStartPlaying = false;
    //[SerializeField] private List<SongsData> allSongs = new List<SongsData>();
    //public int currentSong { get; private set; } = 0;
    [SerializeField] private SongsData levelSongData;
    
    
    private void OnEnable()
    {
        //BeatManager.OnHalfBeat += HalfBeatAction;
        BeatManager.OnPlay += OnPlayEvent;
        BeatManager.OnStop += OnStopEvent;
        //LevelController.OnPauseEvent += OnPauseEventReceiver;
    }

    private void OnDisable()
    {
        //BeatManager.OnHalfBeat -= HalfBeatAction;
        BeatManager.OnPlay -= OnPlayEvent;
        BeatManager.OnStop -= OnStopEvent;
        //LevelController.OnPauseEvent -= OnPauseEventReceiver;
    }
    public void Start()
    {
        //BeatManager.Instance.SetAudioSource(allSongs[currentSong],BeatManager.Instance.CurrentAudioSourceIndex);
        //BeatManager.Instance.SetAudioSource(allSongs[currentSong+1],BeatManager.Instance.CurrentAudioSourceIndex+1);
        if(shouldStartPlaying && !BeatManager.Instance.IsPlaying()) PlayLevelSong();
        
    }

    public void PlayLevelSong(int index=0)
    {
        BeatManager.Instance.PlaySongData(levelSongData);
        
    }
    
    private void OnPlayEvent(float speed)
    {
    }

    private void OnStopEvent(float speed)
    {
        
    }
}
