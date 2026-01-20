using UnityEngine;


public enum SeguridadState
{
    Normal,
    Insecure,
    Flow
}
public class PlayerManager : DanceBrain
{
    public int lifes =3;
    [SerializeField] [Range(0f,1f)] private float NivelDeSeguridad = 0.5f;
    [SerializeField] [Range(0f,0.25f)] private float Alza = 0.1f;
    public RhythmPuzzle targetPuzzle;
    public DanceStep saveDanceStep { get; private set; }
    

    public void GetFlowDamage(bool danho=true)
    {
        
        saveDanceStep = DanceStep.None;
        float value =Mathf.Clamp((danho)?NivelDeSeguridad+Alza:NivelDeSeguridad-Alza,0f,1f);
        if (value == 0)
        {
            //Restar un corazon y hacer invulnerable
            targetPuzzle.PlayerLeave(this);
            lifes = -1;
            DanceLevelController.Instance.UpdateLifesPlayer(lifes);
        }
        else
        {
            NivelDeSeguridad = value;
        }
        DanceLevelController.Instance.UpdateFlowBars(NivelDeSeguridad);
        
    }

    public void GetLifeDamage(bool danho=true)
    {
        lifes += (danho) ? -1 : 1;
    }

    public void AddTargetPuzzle(RhythmPuzzle puzzle)
    {
        targetPuzzle = puzzle;
    }
    public void RemoveTargetPuzzle(RhythmPuzzle puzzle)
    {
        if(puzzle == targetPuzzle)targetPuzzle = null;
    }

    public override void OnDance(DanceStep step)
    {
        saveDanceStep = step;
    }
    
    
}

