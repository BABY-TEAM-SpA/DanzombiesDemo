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
    
    public RhythmPuzzle targetPuzzle;
    public DanceStep saveDanceStep { get; private set; }
    
    public void Start(){
        LevelUIController.Instance?.UpdateFlowBars(NivelDeSeguridad, targetPuzzle!=null);
        if (ActivateOnStart) ActivatePlayer();
    }

    public float GetFlowDamage(bool danho=true)
    {
        saveDanceStep = DanceStep.None;
        float value =Mathf.Clamp((danho)?NivelDeSeguridad-GameManager.Alza:NivelDeSeguridad+GameManager.Alza,-1f,10f);
        NivelDeSeguridad = value;
        LevelUIController.Instance?.UpdateFlowBars(NivelDeSeguridad, targetPuzzle!=null);
        return value;
    }

    public void GetLifeDamage(bool danho=true)
    {
        lifes += (danho) ? -1 : -1;
        PlayerUIController.Instance?.UpdateLifesPlayer(lifes);
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

