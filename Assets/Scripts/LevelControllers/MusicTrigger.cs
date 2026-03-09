using System;
using UnityEngine;

public class MusicTrigger : MusicLevelController
{
    public bool used;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player" && !used)
        {
            used = true;
            SetPlayMusic();
        }
    }
}
