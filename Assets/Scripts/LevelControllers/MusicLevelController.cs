using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicLevelController : MonoBehaviour
{
    
    [SerializeField] private bool shouldStartPlaying = false;
    //[SerializeField] private List<SongsData> allSongs = new List<SongsData>();
    //public int currentSong { get; private set; } = 0;
    [SerializeField] private SongsData levelSongData;
    
    /*
    private void OnEnable()
    {
        AudioManager.OnPlay += OnPlayEvent;
        AudioManager.OnStop += OnStopEvent;
    }

    private void OnDisable()
    {
        AudioManager.OnPlay -= OnPlayEvent;
        AudioManager.OnStop -= OnStopEvent;
    }
	*/

    public void Start()
    {
        if(shouldStartPlaying && !AudioManager.Instance.IsPlaying()) PlayLevelSong();        
    }

    public void PlayLevelSong(int index=0)
    {
        AudioManager.Instance.PlaySongData(levelSongData);
    }
    
    private void OnPlayEvent()
    {
    }

    private void OnStopEvent()
    {
        
    }
}
