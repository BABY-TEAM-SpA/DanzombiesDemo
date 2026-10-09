using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggeredCamera : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private CinemachineStateDrivenCamera sdCamera;
    [SerializeField] private CinemachineCamera[] cameras;

    public CinemachineCamera ActiveCamera => sdCamera?.LiveChild as CinemachineCamera;

    public Action OnCameraActivated;
    public Action OnCameraDeactivated;
    #endregion

    #region [UNITY]
    private void OnEnable() => OnCameraActivated?.Invoke();
    private void OnDisable() => OnCameraDeactivated?.Invoke();
    #endregion

    #region [METHODS]
    #region Setup
    public void Prepare(Animator playerAnimator)
    {
        sdCamera.AnimatedTarget = playerAnimator;
        foreach (CinemachineCamera camera in cameras)
            camera.Follow = playerAnimator.transform;
        SetInstructions();
    }
    #endregion

    #region API
    public void FollowPlayer(Animator playerAnimator)
    {
        Debug.LogError("[PlayerTriggeredCamera] FollowPlayer() deprecated.");

        //sdCamera.AnimatedTarget = playerAnimator;
        //SetInstructions();

        //foreach (CinemachineCamera camera in cameras)
        //    camera.Follow = playerAnimator.transform;
        //sdCamera.Priority = FOLLOW_PRIORITY;

        //OnPlayerFollowed?.Invoke(this);
    }

    public void UnfollowPlayer()
    {
        Debug.LogError("[PlayerTriggeredCamera] UnfollowPlayer() deprecated.");

        /*
        stateDrivenCamera.AnimatedTarget = null;
        foreach (CinemachineCamera camera in cameras)
            camera.Follow = null;
        */
        //sdCamera.Priority = IDLE_PRIORITY;

        //OnPlayerUnfollowed?.Invoke(this);
    }
    #endregion

    #region Helpers
    public Vector2 GetAverageCameras()
    {
        Vector2 center = Vector2.zero;
        foreach (CinemachineCamera camera in cameras)
            center += (Vector2)camera.transform.position;

        return center / cameras.Length;
    }

    private void SetInstructions()
    {
        CinemachineStateDrivenCamera.Instruction[] instructions = sdCamera.Instructions;
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

        sdCamera.Instructions = instructions;
    }
    #endregion
    #endregion
}
