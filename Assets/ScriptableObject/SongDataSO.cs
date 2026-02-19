using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongDataSO", menuName = "Scriptable Objects/SongDataSO")]
public class SongDataSO : ScriptableObject
{
    public string songName;
    public AudioClip clip;
    public int bpm;
    public bool loopeable;
    public List<int> cutFlags = new List<int>();
}
