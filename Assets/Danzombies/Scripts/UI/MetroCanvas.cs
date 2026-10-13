using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputStateTracker
{
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private UnityEvent onReleased;
    private bool state;

    public bool Check(bool currentState)
    {
        if (state != currentState)
        {
            if (currentState) onPressed?.Invoke();
            else onReleased?.Invoke();
            state = currentState;
        }
        return currentState;
    }
}

public class MetroCanvas : MonoBehaviour
{
    [SerializeField] private InputActionReference sprintRef;
    [SerializeField] private InputStateTracker sprintTracker;

    [SerializeField] private InputActionReference moveRef;
    [SerializeField] private InputStateTracker moveTracker;

    public UnityEvent OnSprintPlusMove;

    private void LateUpdate()
    {
        bool sprinting = sprintTracker.Check(sprintRef.action.IsPressed());
        bool moving = moveTracker.Check(moveRef.action.ReadValue<Vector2>().x > 0);

        if (sprinting && moving)
            OnSprintPlusMove?.Invoke();
    }
}
