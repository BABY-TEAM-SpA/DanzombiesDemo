using UnityEngine;
using System;


public enum SeguridadState
{
    Normal,
    Insecure,
    Flow
}

[Serializable]
public class PlayerManager : DanceBrain
{
    [SerializeField]private bool ActivateOnStart;
    public int lifes =3;
    [SerializeField] [Range(0,10)] private float NivelDeSeguridad = 5;
    [SerializeField] [Range(0f,2f)] private float Alza = 1f;
    public RhythmPuzzle targetPuzzle;
    public DanceStep saveDanceStep { get; private set; }
    
    public void Start(){
        LevelUIController.Instance?.UpdateFlowBars(NivelDeSeguridad, targetPuzzle!=null);
        if (ActivateOnStart) ActivatePlayer();
    }

    public void GetFlowDamage(bool danho=true)
    {
        saveDanceStep = DanceStep.None;
        Debug.Log(danho?"Bajando Seguridad":"" );
        float value =Mathf.Clamp((danho)?NivelDeSeguridad-Alza:NivelDeSeguridad+Alza,-1f,10f);
        if (value <= Alza)
        {
            NivelDeSeguridad = value;
            //Restar un corazon y hacer invulnerable
            targetPuzzle.PlayerLeave(this);
            GetLifeDamage(true);
            PlayerUIController.Instance?.UpdateLifesPlayer(lifes);
        }
        else
        {
            NivelDeSeguridad = value;
        }
        LevelUIController.Instance?.UpdateFlowBars(NivelDeSeguridad, targetPuzzle!=null);
        
    }

    public void GetLifeDamage(bool danho=true)
    {
        Debug.Log($"vida dañada");
        lifes += (danho) ? -1 : -1;
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

    public void ActivatePlayer()
    {
        isActive=true;
        EnableMovement(true);
        EnableDance(true);
        beatReciever.isActive = true;
    }

    public void DesactivatePlayer()
    {
        isActive=false;
        EnableMovement(false);
        EnableDance(false);
        beatReciever.isActive = false;
    }
    
    
}

