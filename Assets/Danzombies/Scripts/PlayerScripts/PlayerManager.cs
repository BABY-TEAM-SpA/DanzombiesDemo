using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class PlayerManager : DanceBrain
{
    #region [VARIABLES]
    [Header("PlayerManager")]
    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private PlayerComboController comboController;
    [SerializeField] private PlayerInteractionController interactionController;
    [SerializeField] private bool isTutorial;

    #region Instance
    public static PlayerManager Player;
    public static event Action<BeatReciever.BeatFeedback> DanceFeedbackEvent;
    #endregion

    #region HP, Safe Zone & Combo
    private const int MAX_HP = 3;
    public int HP => hp;
    private int hp = MAX_HP;

    public bool IsSafe => isInSafeZone;
    private bool isInSafeZone;

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
    }

    public void ApplyDanceFeedback(BeatReciever.BeatFeedback bf)
    {
        if (bf == BeatReciever.BeatFeedback.Bad)
        {
            if (movementController.IsRunning && !IsSafe)
                inputController.DisableMoveForSeconds(0.5f); // <- [Frco] Hardcodeado D: y parcheado :D
        }

        LevelProgressTracker.Instance?.RegisterStep(bf);
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
        Reset();
        OnPlayerDeath?.Invoke();
    }
    #endregion

    #region Input
    public void InputSprint(bool isSprinting)
    {
        movCtrl.SetRun(isSprinting);
    }

    public void InputDance(DanceLean lean, DanceDirection direction)
    {
        if(isTutorial) danceAnimCtrl.animator.SetBool("PrepareDance", false);
        if (lean != DanceLean.None && direction != DanceDirection.None)
        {
            DanceStep step = Enum.Parse<DanceStep>(lean + "_" + direction);
            OnDanceStepAction(BeatManager.Instance?BeatManager.Instance.globalBeatCount:1,BeatManager.BeatType.FullBeat, step);
        }
    }

    public void InputInteract() => interactionController.Interact();
    #endregion

    public void Reset()
    {
        hp = MAX_HP;
        comboController?.Reset();
    }

    public Animator ConfinePlayerCamera() => GetComponent<Animator>();
    #endregion
}