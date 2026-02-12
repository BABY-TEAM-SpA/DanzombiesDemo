using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BeatType
{
    Negra,
    Blanca,
    Redonda
}

public class BeatManager : MonoBehaviour
{
    
    public bool ActiveOnStart = false;

    [Header("Sincronización")] /////////////////////////////////////////////////////////
    [Range(0f, 0.4f)] public double margen = 0.25d;
    public bool onMargen { get; private set; }
    public double beatDuration { get; private set; } = 0d;
    public int counter { get; private set; } = 0;

    private bool canPre =true;
    private bool canBeat;
    private bool canPost;
    
    [Header("Eventos por Inspector")] /////////////////////////////////////////////////////////
    public UnityEvent<int> onPreBeatInspector;
    public UnityEvent<int> onBeatInspector;
    public UnityEvent<int> onPostBeatInspector;
    public delegate void OnBeatEvent(int counter);
    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;
    public static BeatManager Instance { get; private set; }
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        if(ActiveOnStart) ResetBeatManager();
    }

    void Update()
    {
        if (AudioManager.Instance.IsPlaying())
        {
            double currentSongTime = AudioSettings.dspTime - AudioManager.Instance.dspSongStartTime;
            beatDuration = AudioManager.Instance.beatDuration;

            // Comprobación de eventos
            if (currentSongTime >= (( beatDuration * counter) - beatDuration * margen) && canPre)
            {
                PreBeat();
            }
            else if (currentSongTime >= beatDuration * counter && canBeat)
            {
                Beat();
            }
            else if (currentSongTime >= ((beatDuration * counter) + beatDuration * margen) && canPost)
            {
                PostBeat();
            }
        }
    }

    void PreBeat()
    {
        canPre = false;
        onMargen = true;
        OnPreBeat?.Invoke((counter));
        onPreBeatInspector?.Invoke((counter));

        canBeat = true;
    }

    void Beat()
    {
        canBeat = false;
        //Debug.Log("Beat " +counter+" at "+ AudioSettings.dspTime.ToString());
        OnBeat?.Invoke((counter));
        onBeatInspector?.Invoke((counter));
        canPost = true;
        
    }

    void PostBeat()
    {
        onMargen = false;
        OnPostBeat?.Invoke((counter-1));
        onPostBeatInspector?.Invoke((counter-1));

        counter += 1;
        canPost = false;
        canPre = true;
    }

    public void ResetBeatManager()
    {
        onMargen = false;
        canPre =false;
        canBeat = true;
        canPost = false;
        counter = 0;
        beatDuration = 0d;
    }
}