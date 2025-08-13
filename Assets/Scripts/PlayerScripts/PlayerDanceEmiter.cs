using UnityEngine;

public class PlayerDanceEmiter : MonoBehaviour
{
    [SerializeField] PlayerAnimatorController playerAnimator;

    private void OnDanceBegin(int danceIndex)
    {
        playerAnimator.OnDanceBegin(danceIndex);
    }

    public void OnStandAction()
    {
        playerAnimator.OnStandAction();
    }

}
