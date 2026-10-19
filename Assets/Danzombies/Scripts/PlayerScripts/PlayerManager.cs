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

    #region Expositions
    public PlayerInputController InputCtrl => inputController;
    public PlayerMovementController MovementCtrl => movementController;
    public PlayerComboController ComboCtrl => comboController;
    public PlayerInteractionController InteractionCtrl => interactionController;
    #endregion

    #region HP & Safe Zone
    private const int MAX_HP = 3;
    public int HP => hp;
    private int hp = MAX_HP;

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
    /// <summary>
    /// Ejecución del paso de baile.
    /// </summary>
    /// <param name="beatCounter"></param>
    /// <param name="beatType">La partición del bit [0, 5]</param>
    /// <param name="beatTimeMs"></param>
    /// <param name="danceStep">El paso de baile (e.g. L_West).</param>
    public override void OnDanceStepAction(int beatCounter, BeatManager.BeatType beatType, int beatTimeMs, DanceStep danceStep)
    {
        onDance?.Invoke(beatType,danceStep);
        danceAnimCtrl?.OnDanceBegin(danceStep);
        if (danceTarget == null) return;
        danceTarget.SetPlayerInput(beatTimeMs,danceStep, out BeatReciever.BeatFeedback bf);
    }

    public void ApplyDanceFeedback(BeatReciever.BeatFeedback bf)
    {
        comboController.Increase(bf, 1);
        if (bf == BeatReciever.BeatFeedback.Bad)
        {
            if (movementController.IsRunning && !IsSafe)
                inputController.DisableMoveForSeconds(BeatManager.Instance.BeatDurationMs/1000f); // <- [Frco] Hardcodeado D: y parcheado :D
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

    /// <summary>
    /// Input de baile (e.g. clic izq. + D = L_East)
    /// </summary>
    public void InputDance(DanceLean lean, DanceDirection direction)
    {
        if (isTutorial) danceAnimCtrl.animator.SetBool("PrepareDance", false);
        if (lean != DanceLean.None && direction != DanceDirection.None)
        {
            DanceStep step = Enum.Parse<DanceStep>(lean + "_" + direction);
            OnDanceStepAction(
                BeatManager.Instance ? BeatManager.Instance.localBeatCount : 1,
                BeatManager.Instance.mainBeatState,
                AudioManager.Instance.SongPositionMs(),
                step
            );
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