using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggeredCamera : MonoBehaviour
{
    #region [VARIABLES]
    private const int ACTIVE_PRIORITY = 1;
    private const int INACTIVE_PRIORITY = 0;
    private Animator targetAnimator;

    [SerializeField] private CinemachineStateDrivenCamera stateDrivenCamera;
    [SerializeField] private CinemachineCamera[] cameras;
    #endregion

    #region [UNITY]
    private void Start() => stateDrivenCamera.Priority = INACTIVE_PRIORITY;

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out  targetAnimator))
        {
            FollowPlayer(targetAnimator);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")  && other.TryGetComponent(out  targetAnimator))
            stateDrivenCamera.Priority = INACTIVE_PRIORITY;
    }
    #endregion
    #endregion

    #region [METHODS]
    private void FollowPlayer(Animator playerAnimator)
    {
        stateDrivenCamera.AnimatedTarget = playerAnimator;
        SetInstructions();
        foreach (CinemachineCamera camera in cameras) camera.Follow = playerAnimator.transform;
        stateDrivenCamera.Priority = ACTIVE_PRIORITY;
    }

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
