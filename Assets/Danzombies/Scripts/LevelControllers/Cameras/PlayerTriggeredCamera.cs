using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggeredCamera : MonoBehaviour
{
    #region [VARIABLES]
    private const int FOLLOW_PRIORITY = 1;
    private const int IDLE_PRIORITY = 0;
    private Animator targetAnimator;

    [SerializeField] private CinemachineStateDrivenCamera stateDrivenCamera;
    [SerializeField] private CinemachineCamera[] cameras;

    public CinemachineCamera ActiveCamera => stateDrivenCamera?.LiveChild as CinemachineCamera;

    public Action<PlayerTriggeredCamera> OnPlayerFollowed;
    public Action<PlayerTriggeredCamera> OnPlayerUnfollowed;
    #endregion

    #region [UNITY]
    private void Start() => stateDrivenCamera.Priority = IDLE_PRIORITY;

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out targetAnimator))
            FollowPlayer(targetAnimator);
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player") && other.TryGetComponent(out targetAnimator))
    //        UnfollowPlayer();
    //}
    #endregion
    #endregion

    #region [METHODS]
    #region API
    public void FollowPlayer(Animator playerAnimator)
    {
        stateDrivenCamera.AnimatedTarget = playerAnimator;
        SetInstructions();

        foreach (CinemachineCamera camera in cameras)
            camera.Follow = playerAnimator.transform;
        stateDrivenCamera.Priority = FOLLOW_PRIORITY;

        OnPlayerFollowed?.Invoke(this);
    }

    public void UnfollowPlayer()
    {
        stateDrivenCamera.AnimatedTarget = null;

        foreach (CinemachineCamera camera in cameras)
            camera.Follow = null;
        stateDrivenCamera.Priority = IDLE_PRIORITY;

        OnPlayerUnfollowed?.Invoke(this);
    }

    public float GetAverageCamerasX()
    {
        float center = 0f;
        foreach (CinemachineCamera camera in cameras)
            center += camera.transform.position.x;

        return center / cameras.Length;
    }
    #endregion

    private void SetInstructions()
    {
        CinemachineStateDrivenCamera.Instruction[] instructions = stateDrivenCamera.Instructions;
        if (instructions.Length < 2)
            return;

        instructions[0] = new CinemachineStateDrivenCamera.Instruction
        {
            FullHash = Animator.StringToHash("LeftLooking"),
            Camera = instructions[0].Camera,
            ActivateAfter = instructions[0].ActivateAfter,
            MinDuration = instructions[0].MinDuration
        };

        instructions[1] = new CinemachineStateDrivenCamera.Instruction
        {
            FullHash = Animator.StringToHash("RightLooking"),
            Camera = instructions[1].Camera,
            ActivateAfter = instructions[1].ActivateAfter,
            MinDuration = instructions[1].MinDuration
        };

        stateDrivenCamera.Instructions = instructions;
    }
    #endregion
}
