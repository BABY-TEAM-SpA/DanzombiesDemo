using UnityEngine;
using UnityEngine.Events;

public class DanceEventEmiter : MonoBehaviour
{
    public UnityEvent onNorth;
    public UnityEvent onSouth;
    public UnityEvent onWest;
    public UnityEvent onEast;
    
    public void OnDanceBuffer(DanceStep danceStep)
    {
        if (danceStep == DanceStep.None) return;
        switch (danceStep)
        {
            case DanceStep.L_North :
                onNorth?.Invoke();
                break;
            case DanceStep.R_North:
                onNorth?.Invoke();
                break;
            case DanceStep.L_South:
                onSouth?.Invoke();
                break;
            case DanceStep.R_South:
                onSouth?.Invoke();
                break;
            case DanceStep.R_West:
                onWest?.Invoke();
                break;
            case DanceStep.L_West:
                onWest?.Invoke();
                break;
            case DanceStep.L_East:
                onEast?.Invoke();
                break;
            case DanceStep.R_East:
                onEast?.Invoke();
                break;
        }
    }
}
