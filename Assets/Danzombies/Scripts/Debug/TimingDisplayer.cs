using UnityEngine;

public class TimingDisplayer : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteComp;
    [SerializeField] RhythmPuzzle target;

    [SerializeField] Color earlyColor;
    [SerializeField] Color lateColor;
    [SerializeField] Color missColor;
   

    void Update(){
        if (target == null) return;
        if (spriteComp == null) return;
        switch (target.currentBeatTiming){
            case Timing.Early:
                spriteComp.color = earlyColor;
            break;
            case Timing.Late:
                spriteComp.color = lateColor;
            break;
            case  Timing.Miss:
                spriteComp.color = missColor;
            break;
        }
    }
}
