using UnityEngine;
using System;
using Unity.Cinemachine;
using UnityEngine.Events;


public enum SeguridadState
{
    Normal,
    Insecure,
    Flow
}

[Serializable]
public class PlayerManager : DanceBrain
{
    [SerializeField] private bool ActivateOnStart;
    public int lifes = 3;
    [SerializeField] [Range(0, 10)] private int nivelDeSeguridad = 5;
    public DanceBarController danceBar{ get; set; } 

    public RhythmPuzzle targetPuzzle{ get; set; }
    
    public UnityEvent LifeDamagedEvent;
    public UnityEvent LifeHealedEvent;
    public static event Action<BeatFeedback> DanceFeedbackEvent;
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
        
        if (targetPuzzle != null)
        {
            DanceStep puzzleStep = targetPuzzle.PlayerMakeDanceStep();
            BeatFeedback bf = BeatManager.Instance.EvaluateInput();
            if (step == puzzleStep)
            {
                if (debug) Debug.Log(bf);
                DanceFeedbackEvent?.Invoke(bf);
            }
            else
            {
                ReportWrongDance();
            }
            
        }
    }

    public void ReportWrongDance()
    {
        DanceFeedbackEvent?.Invoke(BeatFeedback.Bad);
    }
    
    public int GetFlowDamage(int damage)
    {
        int value = Mathf.Clamp(nivelDeSeguridad-(GameManager.Alza*damage),0,10);
        nivelDeSeguridad = value;
        danceBar?.UpdateFlowBars(nivelDeSeguridad, targetPuzzle!=null);
        return value;
    }

    public void GetLifeDamage(bool danho=true)
    {
        if (danho)
        {
            LifeDamagedEvent?.Invoke();
        }
        else
        {
            LifeHealedEvent?.Invoke();
        }
        lifes += (danho) ? -1 : -1;
        PlayerUIController.Instance?.UpdateLifesPlayer(lifes);
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

    public Animator ConfinePlayerCamera()
    {
        return playerAnimCtrl.animator;
    }
}

