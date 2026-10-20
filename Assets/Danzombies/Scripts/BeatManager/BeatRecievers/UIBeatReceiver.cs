using System;
using UnityEngine;
using UnityEngine.Events;

public class UIBeatReceiver : BeatReciever
{
    #region [VARIABLES]
    [SerializeField] private bool activateOnStart = true;
    [SerializeField] private BeatManager.BeatType beatType;
    
    [SerializeField] private UnityEvent OnBeat;
    
    [SerializeField] private Animator animator;
    #endregion

    #region [UNITY]
    private void Start() => SetActive(activateOnStart);
    #endregion

    #region [GODOT]
    #region Beat Receiver
    
        
    public override void OnUpdateSongAction(double barDuration)
    {
        if (animator == null) return;
        animator.enabled = true;
        double duration = barDuration;
        float value = (float)(1f / duration);
        animator.SetFloat("Beat", value);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type, int beatTimeMs)
    {
        if (type == beatType)
            OnBeat?.Invoke();
    }
    
    #endregion
    #endregion
}
