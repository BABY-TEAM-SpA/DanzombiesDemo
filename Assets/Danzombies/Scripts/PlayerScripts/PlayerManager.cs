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
    [SerializeField] private PlayerInteractionController interactionController;
    [SerializeField] private bool isTutorial;

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
        Debug.Log("Adding target puzzle");
        if (danceTarget != null)
        {
            Debug.Log("has target puzzle");
            DanceZone oldTarget = danceTarget;
            danceTarget = target;
            if (target != danceTarget) oldTarget?.PlayerLeave(this);
        }
        else
        {
            Debug.Log("hasnt target puzzle");
            danceTarget = target;
            flowController.Activate();
        }
        danceTarget = target;
    }
    
    public bool TryGetTargetPuzzle(out DanceZone target)
    {
        bool hasTargetZone = danceTarget != null;
        target= hasTargetZone?danceTarget:null ;
        return hasTargetZone;
    }
    
    public void RemoveTargetPuzzle(DanceZone target)
    {
        if (target != danceTarget) return;
        danceTarget = null;
        flowController.Deactivate();
    }
    #endregion

    #region RhythmPuzzle - Dance
    public override void OnDanceStepAction(int beat, BeatManager.BeatType beatType, DanceStep step)
    {
        onDance?.Invoke(step);
        danceAnimCtrl?.OnDanceBegin(step);
        if (danceTarget == null) return;
        danceTarget.SetPlayerInput(step, out BeatReciever.BeatFeedback bf);
        comboController.Increase(bf, 1);
        ApplyDanceFeedback(bf);
    }

    public void ApplyDanceFeedback(BeatReciever.BeatFeedback bf)
    {
        bool hasTargetZone = danceTarget != null;
        DamageMode dmgMode = hasTargetZone
            ? danceTarget.GetDamageMode()
            : DamageMode.None;
        flowController.ApplyFeedback(bf, dmgMode == DamageMode.None);
        DanceFeedbackEvent?.Invoke(bf);
    }
    #endregion

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
        hp = MAX_HP;
        flowController?.SetDefaultFlow();
        comboController?.Reset();

        OnPlayerDeath?.Invoke();
    }
    #endregion

    public Animator ConfinePlayerCamera()
    {
        return danceAnimCtrl.animator;
    }
    #endregion
    
    public void InputDance(DanceLean lean, DanceDirection direction)
    {
        if(isTutorial) danceAnimCtrl.animator.SetBool("PrepareDance",false);
        if (lean != DanceLean.None && direction != DanceDirection.None)
        {
            DanceStep step = Enum.Parse<DanceStep>( lean + "_" + direction );
            OnDanceStepAction(BeatManager.Instance?BeatManager.Instance.globalBeatCount:1,BeatManager.BeatType.FullBeat, step);
        }
    }
    public void InputSprint(bool isSprinting)
    {
        movCtrl.SetRun(isSprinting);
    }

    public void InputInteract() => interactionController.Interact();
}