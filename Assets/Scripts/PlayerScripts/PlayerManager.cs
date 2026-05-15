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
    private int vidas = 3;
    public int lifes => vidas;
    [SerializeField] [Range(0, 10)] private int nivelDeSeguridad = 5;
    public int flow =>nivelDeSeguridad;
    public DanceBarController danceBar{ get; set; } 

    public RhythmPuzzle targetPuzzle{ get; set; }
    
    public UnityEvent LifeDamagedEvent;
    public UnityEvent LifeHealedEvent;
    public static event Action<BeatReciever.BeatFeedback> DanceFeedbackEvent;
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
        danceBar?.UpdateFlowBars(nivelDeSeguridad);
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
            var resultado  = targetPuzzle.PlayerMakeDance();
            if (resultado.Item1)
            {
                if (step == resultado.Item2)
                {
                    BeatReciever.BeatFeedback bf = BeatManager.Instance.EvaluateInput(resultado.Item3);
                    if (debug) Debug.Log(bf);
                    DanceImpact(bf);
                    DanceFeedbackEvent?.Invoke(bf);
                }
                else
                {
                    ReportWrongDance();
                }
            }
        }
    }

    private void DanceImpact(BeatReciever.BeatFeedback bf)
    {
        switch (bf)
        {
            case BeatReciever.BeatFeedback.Perfect:
                IncreaseFlow(2);
                break;
            case BeatReciever.BeatFeedback.Great:
                IncreaseFlow(1);
                break;
            case BeatReciever.BeatFeedback.Early:
                IncreaseFlow(0);
                break;
            case BeatReciever.BeatFeedback.Late:
                IncreaseFlow(0);
                break;
            case BeatReciever.BeatFeedback.Bad:
                IncreaseFlow(-1);
                break;
        }
    }

    public void ReportWrongDance()
    {
        DanceImpact(BeatReciever.BeatFeedback.Bad);
        DanceFeedbackEvent?.Invoke(BeatReciever.BeatFeedback.Bad);
    }

    public int IncreaseFlow(int increment)
    {
        int value = Math.Clamp(nivelDeSeguridad+(GameManager.Alza*increment),0,10);
        SetFlow(value);
        danceBar?.UpdateFlowBars(nivelDeSeguridad);
        if (value < GameManager.Alza)
        {
            GetLifeDamage(true);
            targetPuzzle.PlayerLeave(this);
            SetFlow(5);
        }
        return value;
    }
    
    private void SetFlow(int value)
    {
        nivelDeSeguridad = value;
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
        vidas += (danho) ? -1 : 1;
        vidas = Math.Clamp(lifes,0,3);
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
        return danceAnimCtrl.animator;
    }
}

