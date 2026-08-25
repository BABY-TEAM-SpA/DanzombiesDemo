using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class PlayerManager : DanceBrain
{
    #region [VARIABLES]
    [Header("PlayerManager")]
    [SerializeField] private PlayerFlowController flowController;
    [SerializeField] private PlayerComboController comboController;

    #region Instance
    public static PlayerManager Player;
    public static event Action<BeatReciever.BeatFeedback> DanceFeedbackEvent;
    #endregion

    #region HP
    private const int MAX_HP = 3;
    public int HP => hp;
    private int hp = MAX_HP;
    #endregion

    #region Flow
    public FlowState FlowState => flowController.State;
    public int FlowValue => flowController.Flow;
    public int MaxFlow => flowController.MaxFlow;
    #endregion

    #region Combo
    public ComboState ComboState => comboController.State;
    public int ComboCount => comboController.Count;
    #endregion

    #region SafeZone
    public bool IsSafe => isInSafeZone;
    private bool isInSafeZone;
    #endregion

    #region Events
    [Header("Life Events")]
    public UnityEvent LifeDamagedEvent;
    public UnityEvent LifeHealedEvent;

    public Action OnPlayerDeath;
    #endregion

    [Header("Puzzle")]
    public DanceZone danceTarget;
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
    #region RhythmPuzzle - Puzzle
    public void AddTargetPuzzle(DanceZone target)
    {
        if(target != danceTarget) danceTarget?.PlayerLeave(this);
        danceTarget = target;
        ActivateDanceHUD(true);
    }

    public void ActivateDanceHUD(bool activate)
    {
        DanceBarController.DanceBar?.UpdateFlowBars(FlowValue);
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
    #endregion

    #region RhythmPuzzle - Dance
    public override void OnDanceStepAction(int beat,BeatManager.BeatType beatType, DanceStep step)
    {
        if (danceTarget == null)
            return;

        onDance?.Invoke(step);
        danceAnimCtrl?.OnDanceBegin(step);
        danceTarget.SetPlayerInput(step, out BeatReciever.BeatFeedback bf);
        ApplyDanceFeedback(bf);
    }

    public void ApplyDanceFeedback(BeatReciever.BeatFeedback bf)
    {
        DamageMode dmgMode = danceTarget != null
            ? danceTarget.GetDamageMode()
            : DamageMode.None;
        if (dmgMode == DamageMode.None)
            return;

        flowController.ApplyFeedback(bf);
        DanceFeedbackEvent?.Invoke(bf);
    }
    #endregion

    public int IncreaseFlow(int increment)
    {
        if (isInSafeZone && increment < 0)
            increment = 0;

        DamageMode dmgMode = danceTarget !=null? danceTarget.GetDamageMode(): DamageMode.None;
        
        //danceTarget?.React(increment >= 0 ? DancerExpression.ExpressionType.Normal : DancerExpression.ExpressionType.Angry);
        if (dmgMode == DamageMode.None)
            return 0;
        else
        {
            int value = Math.Clamp(FlowValue + (GameManager.Alza * increment), 0, 10);
            flowController.SetFlow(value);
            //targetPuzzle?.ReactToPlayerStatus(nivelDeSeguridad>5?DancerExpression.ExpressionType.Normal:DancerExpression.ExpressionType.Angry);
            DanceBarController.DanceBar?.UpdateFlowBars(FlowValue);
            //if (value < GameManager.Alza && dmgMode == DamageMode.ModificaFlowYDaña) <- [Frco] En teoría deprecado, la HP no es para el baile
            //{
            //    GetLifeDamage(true);
            //    flowController.SetFlow(5);
            //}
            return value;
        }
    }

    public void SetFlow(int value)
    {
        flowController.SetFlow(value);
        DanceBarController.DanceBar?.UpdateFlowBars(FlowValue);
        
    }

    #region HP & SafeZone
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

    public void SetInSafeZone(bool value) => isInSafeZone = value;

    public void GameOver()
    {
        hp = 3; // <- [Frco] Está hardcodeado, convendría generalizar porque no se está
                //    comunicando con el PlayerCanvas, sino a través de OnPlayerDeath
        OnPlayerDeath?.Invoke();
    }
    #endregion

    public Animator ConfinePlayerCamera()
    {
        return danceAnimCtrl.animator;
    }
    #endregion
}