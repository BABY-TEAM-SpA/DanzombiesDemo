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
    
    private void DanceCheckReciever(BeatFeedback feedback)
    {
        switch (feedback)
        {
            case BeatFeedback.Early:
                EventSender(EarlyLateEvent, EarlyLateDanceEvent);
                break;
            case BeatFeedback.Late:
                EventSender(EarlyLateEvent, EarlyLateDanceEvent);
                break;
            case BeatFeedback.Perfect:
                EventSender(PerfectEvent, PerfectDanceEvent);
                break;
            case BeatFeedback.Great:
                EventSender(GreatEvent, GreatDanceEvent);
                break;
            case BeatFeedback.Bad:
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
