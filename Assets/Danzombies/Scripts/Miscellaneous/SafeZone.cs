using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SafeZone : DanceZone
{
    public UnityEvent onPlayerSucces;
    #region [UNITY]
   
    private void OnEnable() => evaluator.OnFeedback += ReactToFeedback;
    private void OnDisable() => evaluator.OnFeedback -= ReactToFeedback;//danceEventBuffer.RemoveAllListeners();
    
    protected virtual void ReactToFeedback(BeatReciever.BeatFeedback bf)
    {
        //Debug.Log("SafeZone ReactToFeedback");
        if(bf!=BeatReciever.BeatFeedback.Bad) playersInside?.ApplyDanceFeedback(bf);
        OnPlayerFeedback?.Invoke(bf);
        if((int)bf>1)
        {
            //Debug.Log("Player Success");
            onPlayerSucces?.Invoke();
        }
    }
    
    #endregion
}
