using System;
using System.Collections.Generic;
using UnityEngine;

public enum ActionToPlaySong
{
    Enqueue,
    Interrupt
}
[Serializable]
public class MusicToQueue
{
    public string songName;
    public ActionToPlaySong actionToPlay = ActionToPlaySong.Enqueue;
}

public class MusicLevelController : MonoBehaviour
{
    [SerializeField] private bool shouldStartPlaying = false;
    [SerializeField] private List<MusicToQueue> LevelSongs = new List<MusicToQueue>();

    public void Start()
    {
        if (shouldStartPlaying)
        {
            SetPlayMusic();
        }  
    }

    public void SetPlayMusic()
    {
        if (LevelSongs.Count > 0)
        {
            for (int i = 0; i < LevelSongs.Count; i++)
            {
                MusicToQueue mtq = LevelSongs[i];
                CallToPLay(mtq);
            }
        }
        
    }

    private void CallToPLay(MusicToQueue mtq)
    {
        AudioManager.Instance?.PlaySong(mtq.songName,mtq.actionToPlay == ActionToPlaySong.Interrupt );
    }
}
