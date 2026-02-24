using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongDataSO", menuName = "Scriptable Objects/SongDataSO")]
public class SongDataSO : ScriptableObject
{
    public string songName;
    public AudioClip clip;
    public bool loopeable;
    public List<int> cutFlags = new List<int>(); // in compases
    public double delayAtEnd = 0d;
    
    [Header("Music Data")]
    public int bpm;
    public int metric;
    public int compassLong;

}
