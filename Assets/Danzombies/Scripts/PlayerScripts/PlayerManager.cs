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

    [Header("Puzzle")]
    public DanceZone danceTarget;

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
    public void AddTargetPuzzle(DanceZone target)
    {
        if(target != danceTarget) danceTarget?.PlayerLeave(this);
        danceTarget = target;
        ActivateDanceHUD(true);
    }

    public void ActivateDanceHUD(bool activate)
    {
        DanceBarController.DanceBar?.UpdateFlowBars(nivelDeSeguridad);
        DanceBarController.DanceBar?.Activate(activate);
    }

    public void RemoveTargetPuzzle(DanceZone target)
    {
        if (target == danceTarget)
        {
            danceTarget = null;
            ActivateDanceHUD(false);
        }
    }

    public override void OnDanceStepAction(int beat,BeatManager.BeatType beatType, DanceStep step)
    {
        if (danceTarget == null) return;
        onDance?.Invoke(step);
        danceAnimCtrl?.OnDanceBegin(step);
        danceTarget.SetPlayerInput(step, out BeatReciever.BeatFeedback bf);
        ApplyDanceFeedback(bf);
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

    public int IncreaseFlow(int increment)
    {
        if (isInSafeZone && increment < 0)
        {
            increment = 0;
            //¡Se limitó la pérdida de Flow porque Grerg está en una zona segura!");
        }

        DamageMode dmgMode = danceTarget !=null? danceTarget.GetDamageMode(): DamageMode.None;
        
        //danceTarget?.React(increment >= 0 ? DancerExpression.ExpressionType.Normal : DancerExpression.ExpressionType.Angry);
        if (dmgMode == DamageMode.None) return 0;
        else
        {

            int value = Math.Clamp(nivelDeSeguridad + (GameManager.Alza * increment), 0, 10);
            SetFlow(value);
            //targetPuzzle?.ReactToPlayerStatus(nivelDeSeguridad>5?DancerExpression.ExpressionType.Normal:DancerExpression.ExpressionType.Angry);
            DanceBarController.DanceBar?.UpdateFlowBars(nivelDeSeguridad);
            if (value < GameManager.Alza && dmgMode == DamageMode.ModificaFlowYDaña)
            {
                GetLifeDamage(true);
                SetFlow(5);
            }
            return value;
        }
        
    }

    private void SetFlow(int value) => nivelDeSeguridad = value;
    public void SetInSafeZone(bool value) => isInSafeZone = value;

    public void GetLifeDamage(bool receiveDamage = true)
    {
        hp += (receiveDamage) ? -1 : 1;
        hp = Math.Clamp(hp, 0, 3);
        //PlayerUIController.Instance?.UpdateLifesPlayer(hp);

        if (receiveDamage)
            LifeDamagedEvent?.Invoke();
        else LifeHealedEvent?.Invoke();

        if (hp <= 0)
            GameOver();
    }

    public void GameOver()
    {
        hp = 3; // <- [Frco] Está hardcodeado, convendría generalizar porque no se está
                //    comunicando con el PlayerCanvas, sino a través de OnPlayerDeath
        OnPlayerDeath?.Invoke();
    }
    
    public Animator ConfinePlayerCamera()
    {
        return danceAnimCtrl.animator;
    }
    #endregion
}