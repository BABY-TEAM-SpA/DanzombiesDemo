using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class PlayerManager : DanceBrain
{
    //definitions
    public class PlayerStepEvent: UnityEvent<PlayerManager, DanceStep>{} //A pesar de que PlayerManager es un singleton lo programo con la posibilidad en mente de que exista mas de uno, por que asi estaba hecho en RhythmPuzzle
    
    [SerializeField] bool debug;

    [SerializeField] PlayerDanceMemory danceMemory; //componente que recuerda los ultimos pasos de baile ejecutados correctamente
    [SerializeField] private bool ActivateOnStart;
    public int lifes =3;
    [SerializeField] [Range(0,10)] private int nivelDeSeguridad = 5;
    public DanceBarController danceBar;
    
    RhythmPuzzle targetPuzzle; //Ssoar: lo hice privado. no rompio nada asi que lo dejo
  
    [Header("Events")]  
    public PlayerStepEvent OnDanceEvent = new PlayerStepEvent();
    public UnityEvent OnTakeDamage;
    
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
        OnDanceEvent.AddListener(danceMemory.RememberStep);
    }

    public void TakeFlowDamage(int damage)
    {
        int totalFlow = Mathf.Clamp(nivelDeSeguridad-(GameManager.Alza*damage),0,10);
        nivelDeSeguridad = totalFlow;
        danceBar?.UpdateFlowBars(nivelDeSeguridad, targetPuzzle!=null);

        if (totalFlow < GameManager.Alza)
        {
            if (debug)Debug.Log("[PlayerManager]: Player quedo sin flow.");
            GetLifeDamage(true);
            
        }
    }

    public int GetFlow(){
        return nivelDeSeguridad;
    }

    public void GetLifeDamage(bool danho=true)
    {
        lifes += (danho) ? -1 : -1;
        PlayerUIController.Instance?.UpdateLifesPlayer(lifes);
        RemoveTargetPuzzle(targetPuzzle);
        OnTakeDamage?.Invoke();
    }

    public bool IsAlreadyTargetPuzzle(RhythmPuzzle puzzleToCheck)
    {
        return puzzleToCheck == targetPuzzle;
    }

    public void AddTargetPuzzle(RhythmPuzzle puzzle)
    {
        danceBar?.Activate(puzzle.activeDanceSequence.flowAffect);
        targetPuzzle = puzzle;
        puzzle.SubscribeToPlayerReactions(this);
    }

    public void RemoveTargetPuzzle(RhythmPuzzle puzzle)
    {
        if (puzzle == targetPuzzle)
        {
            danceBar?.Activate(false);
            targetPuzzle = null;
        }
        puzzle.UnsubscribeToPlayerReactions(this);
    }

    public override void OnDance(DanceStep step)
    {
        if (step != DanceStep.None)
            OnDanceEvent?.Invoke(this, step);
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
    
    public void MissedStep() //this will likely need to be changed completely but have to ask Javier
    {
        TakeFlowDamage(1);
        danceMemory.InitializeMemory();
    }

    public PlayerDanceMemory GetDanceMemory()
    {
        return danceMemory;
    }
}

