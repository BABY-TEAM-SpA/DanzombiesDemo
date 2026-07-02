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

    private int vidas = 3;

    public int lifes => vidas;

    [SerializeField]
    [Range(0, 10)]
    private int nivelDeSeguridad = 5;

    public int flow => nivelDeSeguridad;

    public DanceBarController danceBar { get; set; }
    
    public UnityEvent LifeDamagedEvent;
    public UnityEvent LifeHealedEvent;
    public static event Action<BeatReciever.BeatFeedback> DanceFeedbackEvent;

    public static PlayerManager Player;
    public RhythmPuzzle targetPuzzle;


    private void Awake()
    {
        if (Player == null)
            Player = this;
        else Destroy(gameObject);
    }
    

    public void AddTargetPuzzle(RhythmPuzzle puzzle)
    {
        targetPuzzle = puzzle;
    }

    public void ActivateDanceHUD(bool activate)
    {
        danceBar = GUIManager.Instance?.DanceBar;
        danceBar?.UpdateFlowBars(nivelDeSeguridad);
        danceBar?.Activate(activate);
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
        if (targetPuzzle == null)
            return;
        if (targetPuzzle.SetPlayerInput(step, out BeatReciever.BeatFeedback bf))
        {
            ApplyDanceFeedback(bf);
        }
    }


    public void ApplyDanceFeedback(BeatReciever.BeatFeedback bf)
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
        DanceFeedbackEvent?.Invoke(bf);
    }

    public void ReportEmptyDance()
    {
        ApplyDanceFeedback(BeatReciever.BeatFeedback.Bad);
    }

    public int IncreaseFlow(int increment)
    {
        SequenceStep.SequenceFlowType seqtype =targetPuzzle.currentDanceData.Sequence.sequenceFlowType;
        if (seqtype == SequenceStep.SequenceFlowType.NoFlowAffect_NoHurt) return 0;
        else
        {
            int value = Math.Clamp(nivelDeSeguridad + (GameManager.Alza * increment), 0, 10);
            SetFlow(value);
            danceBar?.UpdateFlowBars(nivelDeSeguridad);
            if (value < GameManager.Alza && seqtype == SequenceStep.SequenceFlowType.FlowAffect_Hurt)
            {
                GetLifeDamage(true);
                targetPuzzle?.PlayerGetDamaged();
                SetFlow(5);
            }
            return value;
        }
    }

    private void SetFlow(int value)
    {
        nivelDeSeguridad = value;
    }


    public void GetLifeDamage(bool danho = true)
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

        vidas = Math.Clamp(lifes, 0, 3);

        PlayerUIController.Instance?.UpdateLifesPlayer(lifes);
    }
    public Animator ConfinePlayerCamera()
    {
        return danceAnimCtrl.animator;
    }
}