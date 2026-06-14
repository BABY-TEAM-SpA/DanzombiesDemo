using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public enum ActionToPlaySong
{
    Enqueue,
    Interrupt
}

[Serializable]
public class MusicToQueue
{
    [Tooltip("Ej: event:/Music/MainTheme")]
    public EventReference  eventPath;

    public ActionToPlaySong actionToPlay =
        ActionToPlaySong.Enqueue;
}

public class MusicLevelController : MonoBehaviour
{
    [SerializeField] private bool shouldStartPlaying;

    [SerializeField]
    private List<MusicToQueue> levelSongs =
        new();

    private void Start()
    {
        if (shouldStartPlaying)
            SetPlayMusic();
    }

    public void SetPlayMusic()
    {
        foreach (MusicToQueue music in levelSongs)
        {
            PlayMusic(music);
        }
    }

    private void PlayMusic(MusicToQueue music)
    {
        AudioManager.Instance?.PlayRhythmSong(music.eventPath, music.actionToPlay == ActionToPlaySong.Interrupt);
    }
}