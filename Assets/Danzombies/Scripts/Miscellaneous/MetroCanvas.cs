using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MetroCanvas : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InputActionReference sprintRef;
    [SerializeField] private InputActionReference moveRef;

    public UnityEvent OnShiftPlusD;
    #endregion

    #region [UNITY]
    private void LateUpdate()
    {
        bool shift = sprintRef.action.WasPressedThisFrame();
        bool d = moveRef.action.ReadValue<Vector2>().x > 0;

        if (shift && d)
            OnShiftPlusD?.Invoke();
    }
    #endregion
}
