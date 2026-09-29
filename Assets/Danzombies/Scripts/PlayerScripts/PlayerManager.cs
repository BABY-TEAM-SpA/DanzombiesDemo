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

    #region Combo & Flow
    public PlayerFlowController FlowCtrl => flowController;
    public PlayerComboController ComboCtrl => comboController;
    #endregion

    #region Events
    [Header("Life Events")]
    public UnityEvent LifeDamagedEvent;
    public UnityEvent LifeHealedEvent;

    public Action OnPlayerDeath;
    #endregion

    [Header("Puzzle")]
    public DanceZone danceTarget;
    public bool HasTarget => danceTarget != null;
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
        if (target == null) return;
        
        DanceZone oldTarget = danceTarget;
        if (oldTarget == target) return;
        
        danceTarget = target;
        oldTarget?.PlayerLeave(this);
        flowController?.BindZone(target);
    }

    public bool TryGetTargetPuzzle(out DanceZone target)
    {
        target = danceTarget;
        return HasTarget;
    }
    
    public void RemoveTargetPuzzle(DanceZone target)
    {
        if (target != danceTarget) return;

        danceTarget = null;
        flowController?.UnbindZone(target);
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
        flowController?.ApplyFeedback(bf);
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

    public void SetInSafeZone(bool value) => flowController?.SetSafe(value);

    public void GameOver()
    {
        hp = MAX_HP;
        flowController?.ResetFlow();
        comboController?.Reset();

        OnPlayerDeath?.Invoke();
    }
    #endregion

    #region Input
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
    #endregion

    public Animator ConfinePlayerCamera() => danceAnimCtrl.animator;
    #endregion
}