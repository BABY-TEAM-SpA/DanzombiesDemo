using System;
using System.Collections.Generic;
using UnityEngine;


public class MusicLevelController : MonoBehaviour
{
    [Serializable]
    class MusicToQueue
    {
        public bool useName;
        public string songName;
        public int songIndex;
    }
    
    [SerializeField] private bool shouldStartPlaying = false;
    [SerializeField] private List<MusicToQueue> LevelSongs = new List<MusicToQueue>();
    public bool ForceStartPlaying { get { return shouldStartPlaying; } set { shouldStartPlaying = value; } }

    public void Start()
    {
        if (shouldStartPlaying)
        {
            SetPlayMusic();
        }  
    }

    public void SetPlayMusic(bool force=false)
    {
        if (LevelSongs.Count > 0)
        {
            for (int i = 0; i < LevelSongs.Count; i++)
            {
                MusicToQueue mtq = LevelSongs[i];
                bool interrupt = (i == 0 && ForceStartPlaying);
                if (mtq.useName) AudioManager.Instance.PlaySong(mtq.songName,interrupt ); else AudioManager.Instance.PlaySong(mtq.songIndex,interrupt);
            }
        }
        
    }
}
