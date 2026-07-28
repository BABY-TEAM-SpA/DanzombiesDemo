using UnityEngine;
using System;
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
    #region [VARIABLES]
    public int HP => hp;
    private int hp = 3;

    public int flow => nivelDeSeguridad;
    [SerializeField][Range(0, 10)] private int nivelDeSeguridad = 5;

    public bool IsSafe => isInSafeZone;
    private bool isInSafeZone;
    
    public UnityEvent LifeDamagedEvent;
    public UnityEvent LifeHealedEvent;
    public static event Action<BeatReciever.BeatFeedback> DanceFeedbackEvent;

    public static PlayerManager Player;
    public RhythmPuzzle targetPuzzle;

    public Action OnPlayerDeath;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if (Player == null)
            Player = this;
        else Destroy(gameObject);
    }

    
    #endregion

    #region [METHODS]
    public void AddTargetPuzzle(RhythmPuzzle puzzle)
    {
        targetPuzzle = puzzle;
        ActivateDanceHUD(true);
    }

    public void ActivateDanceHUD(bool activate)
    {
        DanceBarController.DanceBar?.UpdateFlowBars(nivelDeSeguridad);
        DanceBarController.DanceBar?.Activate(activate);
    }

    public void RemoveTargetPuzzle(RhythmPuzzle puzzle)
    {
        
        if (puzzle == targetPuzzle)
        {
            targetPuzzle = null;
            ActivateDanceHUD(false);
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
        if (isInSafeZone && increment < 0)
        {
            increment = 0;
            Debug.Log($"[PlayerManager] ¡Se limitó la pérdida de Flow porque Grerg está en una zona segura!");
        }

        SequenceStep.DamageMode seqtype = targetPuzzle !=null?targetPuzzle.GetDamageMode(): SequenceStep.DamageMode.None;
        if (seqtype == SequenceStep.DamageMode.None) return 0;
        else
        {

            int value = Math.Clamp(nivelDeSeguridad + (GameManager.Alza * increment), 0, 10);
            SetFlow(value);
            targetPuzzle?.ReactToPlayerStatus(nivelDeSeguridad>5?DancerExpression.ExpressionType.Normal:DancerExpression.ExpressionType.Angry);
            DanceBarController.DanceBar?.UpdateFlowBars(nivelDeSeguridad);
            if (value < GameManager.Alza && seqtype == SequenceStep.DamageMode.ModificaFlowYDaña)
            {
                GetLifeDamage(true);
                targetPuzzle?.PlayerGetDamaged();
                SetFlow(5);
            }
            return value;
        }
        
    }

    private void SetFlow(int value) => nivelDeSeguridad = value;
    public void SetInSafeZone(bool value) => isInSafeZone = value;

    public void GetLifeDamage(bool danho = true)
    {
        if (danho)LifeDamagedEvent?.Invoke();
        else LifeHealedEvent?.Invoke();
        hp += (danho) ? -1 : 1;
        hp = Math.Clamp(hp, 0, 3);
        PlayerUIController.Instance?.UpdateLifesPlayer(hp);
    }
    public void GameOver()
    {
        OnPlayerDeath?.Invoke();
    }
    
    public Animator ConfinePlayerCamera()
    {
        return danceAnimCtrl.animator;
    }
    #endregion
}