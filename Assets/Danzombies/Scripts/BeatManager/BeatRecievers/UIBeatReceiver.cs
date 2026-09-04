using System;
using UnityEngine;
using UnityEngine.Events;

public class UIBeatReceiver : BeatReciever
{
    #region [VARIABLES]
    [SerializeField] private bool activateOnStart = true;

    [SerializeField] private UnityEvent OnPreBeat;
    [SerializeField] private UnityEvent OnBeat;
    [SerializeField] private UnityEvent OnPostBeat;
    #endregion

    #region [UNITY]
    private void Start() => SetActive(activateOnStart);
    #endregion

    #region [GODOT]
    #region Beat Receiver
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        OnPreBeat?.Invoke();
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        Debug.Log($"[UIBeatReceiver] Beat ({beat})");
        OnBeat?.Invoke();
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        OnPostBeat?.Invoke();
    }
    #endregion
    #endregion
}
