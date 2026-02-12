using UnityEngine;
using UnityEngine.Events;

public class DanceEventEmiter : MonoBehaviour
{
    [SerializeField] PlayerAnimatorController playerAnimator;
    public UnityAction OnEnterAnimationEnd;
    
    public void OnEnterAnimationEndEvent()
    {
        OnEnterAnimationEnd?.Invoke();
    }
    
    private void OnDanceBegin(int danceIndex)
    {
        playerAnimator.OnDanceBegin(danceIndex);
    }

    public void OnStandAction()
    {
        playerAnimator.OnStandAction();
    }
}
