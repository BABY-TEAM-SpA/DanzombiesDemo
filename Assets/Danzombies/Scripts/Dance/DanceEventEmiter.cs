using UnityEngine;
using UnityEngine.Events;

public class DanceEventEmiter : MonoBehaviour
{
    public UnityEvent<float> onNorth;
    public UnityEvent<float> onSouth;
    public UnityEvent<float> onWest;
    public UnityEvent<float> onEast;
    
    public void OnDanceBuffer(DanceStep danceStep)
    {
        if (danceStep == DanceStep.None) return;
        switch (danceStep)
        {
            case DanceStep.L_North :
                onNorth?.Invoke(0f);
                break;
            case DanceStep.R_North:
                onNorth?.Invoke(1f);
                break;
            case DanceStep.L_South:
                onSouth?.Invoke(0f);
                break;
            case DanceStep.R_South:
                onSouth?.Invoke(1f);
                break;
            case DanceStep.R_West:
                onWest?.Invoke(0f);
                break;
            case DanceStep.L_West:
                onWest?.Invoke(1f);
                break;
            case DanceStep.L_East:
                onEast?.Invoke(0f);
                break;
            case DanceStep.R_East:
                onEast?.Invoke(1f);
                break;
        }
    }
}
