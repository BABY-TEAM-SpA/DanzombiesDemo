using UnityEngine;
using UnityEngine.Events;

public class DanceEventEmiter : MonoBehaviour
{
    public UnityEvent onL_North;
    public UnityEvent onR_North;
    public UnityEvent onL_South;
    public UnityEvent onR_South;
    public UnityEvent onL_West;
    public UnityEvent onR_West;
    public UnityEvent onL_East;
    public UnityEvent onR_East;
    
    public void OnDanceBuffer(DanceStep danceStep)
    {
        if (danceStep == DanceStep.None) return;
        switch (danceStep)
        {
            case DanceStep.L_North:
                onL_North?.Invoke();
                break;
            case DanceStep.L_South:
                onL_South?.Invoke();
                break;
            case DanceStep.L_West:
                onL_West?.Invoke();
                break;
            case DanceStep.L_East:
                onL_East?.Invoke();
                break;
            case DanceStep.R_North:
                onR_North?.Invoke();
                break;
            case DanceStep.R_South:
                onR_South?.Invoke();
                break;
            case DanceStep.R_West:
                onR_West?.Invoke();
                break;
            case DanceStep.R_East:
                onR_East?.Invoke();
                break;
        }
    }
}
