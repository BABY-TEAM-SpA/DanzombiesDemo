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
    [SerializeField] [Range(0,10)] private int nivelDeSeguridad = 5;
    public DanceBarController danceBar;
    
    public RhythmPuzzle targetPuzzle;
    public DanceStep saveDanceStep { get; private set; }
    
    
    public static PlayerManager Player;

    private void Awake()
    {
        if (Player == null)
        {
            Player = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start(){
        if (ActivateOnStart) ActivatePlayer();
    }

    public int GetFlowDamage(int damage)
    {
        saveDanceStep = DanceStep.None;
        int value = Mathf.Clamp(nivelDeSeguridad-(GameManager.Alza*damage),0,10);
        nivelDeSeguridad = value;
        danceBar.UpdateFlowBars(nivelDeSeguridad, targetPuzzle!=null);
        return value;
    }

    public void GetLifeDamage(bool danho=true)
    {
        lifes += (danho) ? -1 : -1;
        PlayerUIController.Instance?.UpdateLifesPlayer(lifes);
    }

    public void AddTargetPuzzle(RhythmPuzzle puzzle)
    {
        danceBar?.Activate(puzzle.activeDanceSequence.flowAffect);
        targetPuzzle = puzzle;
    }
    public void RemoveTargetPuzzle(RhythmPuzzle puzzle)
    {
        if (puzzle == targetPuzzle)
        {
            danceBar?.Activate(false);
            targetPuzzle = null;
        }
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
        beatReciever.SetActive(true);
    }

    public void DesactivatePlayer()
    {
        isActive=false;
        EnableMovement(false);
        EnableDance(false);
        beatReciever.SetActive(false);
    }
    
    
}

