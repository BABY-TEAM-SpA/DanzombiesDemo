using UnityEngine;
using UnityEngine.Events;

public class DanceFeedbackReciever : MonoBehaviour
{
    public delegate void DanceCheckEvent();
    public event DanceCheckEvent BadEvent;
    public UnityEvent BadDanceEvent;
    public event DanceCheckEvent EarlyLateEvent;
    public UnityEvent EarlyLateDanceEvent;
    public event DanceCheckEvent GreatEvent;
    public UnityEvent GreatDanceEvent;
    public event DanceCheckEvent PerfectEvent;
    public UnityEvent PerfectDanceEvent;
    
    public void OnEnable()
    {
        PlayerManager.DanceFeedbackEvent += DanceCheckReciever;
    }
    public void OnDisable()
    {
        PlayerManager.DanceFeedbackEvent -= DanceCheckReciever;
    }
    
    private void DanceCheckReciever(BeatReciever.BeatFeedback feedback)
    {
        switch (feedback)
        {
            case BeatReciever.BeatFeedback.Early:
                EventSender(EarlyLateEvent, EarlyLateDanceEvent);
                break;
            case BeatReciever.BeatFeedback.Late:
                EventSender(EarlyLateEvent, EarlyLateDanceEvent);
                break;
            case BeatReciever.BeatFeedback.Perfect:
                EventSender(PerfectEvent, PerfectDanceEvent);
                break;
            case BeatReciever.BeatFeedback.Great:
                EventSender(GreatEvent, GreatDanceEvent);
                break;
            case BeatReciever.BeatFeedback.Bad:
                EventSender(BadEvent, BadDanceEvent);
                break;
        }
    }
    public void EventSender( DanceCheckEvent delegateEvent, UnityEvent unityEvent )
    {
        delegateEvent?.Invoke();
        unityEvent?.Invoke();
    }
}
