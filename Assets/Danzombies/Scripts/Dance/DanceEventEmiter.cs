using UnityEngine;
using UnityEngine.Events;

public class DanceEventEmiter : MonoBehaviour
{
    public UnityEvent<string> onNorth;
    public UnityEvent<string> onSouth;
    public UnityEvent<string> onWest;
    public UnityEvent<string> onEast;
    
    public void OnDanceBuffer(DanceStep danceStep)
    {
        if (danceStep == DanceStep.None) return;
        switch (danceStep)
        {
            case DanceStep.L_North :
                onNorth?.Invoke("L");
                break;
            case DanceStep.R_North:
                onNorth?.Invoke("R");
                break;
            case DanceStep.L_South:
                onSouth?.Invoke("L");
                break;
            case DanceStep.R_South:
                onSouth?.Invoke("R");
                break;
            case DanceStep.R_West:
                onWest?.Invoke("L");
                break;
            case DanceStep.L_West:
                onWest?.Invoke("R");
                break;
            case DanceStep.L_East:
                onEast?.Invoke("L");
                break;
            case DanceStep.R_East:
                onEast?.Invoke("R");
                break;
        }
    }
}
