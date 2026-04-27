using System;
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

    [Header("Sincronización")]
    [Range(0f,0.4f)]
    public double margen = 0.25d;

    public bool onMargen { get; private set; }

    public double beatDuration { get; private set; }

    public int counter { get; private set; }

    int lastBeat = -1;
    int lastHalfBeat = -1;

    double dspStartTime;

    bool preTriggered;
    bool beatTriggered;
    bool postTriggered;

    bool preHalfTriggered;
    bool halfTriggered;
    bool postHalfTriggered;
    


    public delegate void OnUpdate(double beatDuration);
    public static event OnUpdate OnUpdateEvent;

    public delegate void OnBeatEvent(int counter);
    public static event OnBeatEvent OnPreBeat;
    public static event OnBeatEvent OnBeat;
    public static event OnBeatEvent OnPostBeat;

    public static event OnBeatEvent OnPreHalfBeat;
    public static event OnBeatEvent OnHalfBeat;
    public static event OnBeatEvent OnPostHalfBeat;

    public static BeatManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        AudioManager.OnPlay += OnPlayEvent;
    }

    void OnDisable()
    {
        AudioManager.OnPlay -= OnPlayEvent;
    }

    void OnPlayEvent(bool resetCounter)
    {
        ResetBeatManager(resetCounter);
        OnUpdateEvent?.Invoke(beatDuration);
    }

    void Update()
    {
        if (!AudioManager.Instance.IsPlaying())
            return;

        double dspTime = AudioSettings.dspTime;

        double songTime =
            dspTime -
            AudioManager.Instance.currentSongPlaying.dspSongStartTime;

        UpdateBeat(songTime);
        UpdateHalfBeat(songTime);
    }

    void UpdateBeat(double songTime)
    {
        int closestBeat = GetClosestBeat(songTime);

        if (closestBeat != lastBeat)
        {
            lastBeat = closestBeat;

            preTriggered = false;
            beatTriggered = false;
            postTriggered = false;

            counter+=1;
        }

        double beatStart = closestBeat * beatDuration;

        if (!preTriggered &&
            songTime >= beatStart - beatDuration * margen)
        {
            preTriggered = true;
            onMargen = true;
            OnPreBeat?.Invoke(closestBeat);
        }

        if (!beatTriggered &&
            songTime >= beatStart)
        {
            beatTriggered = true;

            OnBeat?.Invoke(closestBeat);
        }

        if (!postTriggered &&
            songTime >= beatStart + beatDuration * margen)
        {
            postTriggered = true;
            onMargen = false;

            OnPostBeat?.Invoke(closestBeat);
        }
    }

    void UpdateHalfBeat(double songTime)
    {
        double halfDuration = beatDuration * 0.5;

        int currentHalfBeat = (int)(songTime / halfDuration);

        if (currentHalfBeat != lastHalfBeat)
        {
            lastHalfBeat = currentHalfBeat;

            preHalfTriggered = false;
            halfTriggered = false;
            postHalfTriggered = false;
        }

        double halfStart = currentHalfBeat * halfDuration;

        if (!preHalfTriggered &&
            songTime >= halfStart - halfDuration * margen)
        {
            preHalfTriggered = true;

            OnPreHalfBeat?.Invoke(currentHalfBeat);
        }

        if (!halfTriggered &&
            songTime >= halfStart)
        {
            halfTriggered = true;

            OnHalfBeat?.Invoke(currentHalfBeat);
        }

        if (!postHalfTriggered &&
            songTime >= halfStart + halfDuration * margen)
        {
            postHalfTriggered = true;

            OnPostHalfBeat?.Invoke(currentHalfBeat);
        }
    }

    public void ResetBeatManager(bool resetCounter)
    {
        beatDuration =
            AudioManager.Instance.currentSongPlaying.beatDuration;

        dspStartTime =
            AudioManager.Instance.currentSongPlaying.dspSongStartTime;
        
        preTriggered = false;
        beatTriggered = false;
        postTriggered = false;

        preHalfTriggered = false;
        halfTriggered = false;
        postHalfTriggered = false;

        if (resetCounter)
        {
            Debug.Log("Counter Reset");
            counter = 0;
            lastBeat = -1;
            lastHalfBeat = -1;
        }
    }

    int GetClosestBeat(double songTime)
    {
        //given a current songTime, it spits out which beat is closest. This could be the next or the previous beat. (Ssoar)
        //this *can* give negative beats which would mean the last beat in the song is the closest (Ssoar)
        int nextBeat = (int)(songTime / beatDuration) + 1;
        int prevBeat = (int)(songTime / beatDuration);
        double nextBeatTime = nextBeat * beatDuration;
        double prevBeatTime = prevBeat * beatDuration;
        double nextBeatDistance = Mathf.Abs((float)(songTime - nextBeatTime));
        double prevBeatDistance = Mathf.Abs((float)(songTime - prevBeatTime));
        if (nextBeatDistance < prevBeatDistance)
        {
            return nextBeat;
        }
        else
        {
            return prevBeat;
        }
    }
}