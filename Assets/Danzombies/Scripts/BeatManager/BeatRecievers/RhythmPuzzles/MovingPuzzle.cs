using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPuzzle : RhythmPuzzle
{
    #region [METHODS]
    [Header("Moving Settings")]
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;
    public MovingSequence[] movingSequences;

    private int index = 0;
    private bool isMoving;
    private MovingSequence lastSequence;
    #endregion

    #region [UNITY]
    private void OnDisable() => Disconnect();
    #endregion

    #region [METHODS]
    #region Movement
    private void StartMoving()
    {
        isMoving = true;
        Disconnect();
        Steph.Move(lastSequence.movingDirection, lastSequence.movingDuration, StopMoving);
    }

    private void StopMoving()
    {
        isMoving = false;
        Connect();
    }
    #endregion

    #region RhythmPuzzle - Setup
    public override void PreparePuzzle() => Connect();

    public override void ActivatePuzzle(bool activate)
    {
        base.ActivatePuzzle(activate);
        ShowHUD(activate);

        if (!activate)
            return;

        isMoving = false;
        SetWholeSequence();
    }
    #endregion

    #region RhythmPuzzle - Update
    public override void OnUpdateSongAction() { }
    public override void OnDanceSequenceCleared()
    {
        index++;
        SetWholeSequence();
    }
    #endregion

    #region RythmPuzzle - Player
    public override bool SetPlayerInput(DanceStep playerStep, out BeatFeedback bf)
    {
        bool danced = base.SetPlayerInput(playerStep, out bf);
        bool isCorrect = playerStep == currentDanceData.DanceStep;

        if (danced && isCorrect && !isMoving)
            StartMoving();
        return danced;
    }

    public override void ReactToPlayerStatus(DancerExpression.ExpressionType exp) { }
    public override void PlayerGetDamaged() { }
    #endregion

    #region Helpers
    private void SetWholeSequence()
    {
        if (index < movingSequences.Length)
        {
            lastSequence = movingSequences[index];
            base.SetSequence(lastSequence.danceSequence);
        }
        else ActivatePuzzle(false);
    }

    private void Connect()
    {
        Steph?.Connect(this);

        HUD?.Connect(this);
        ShowHUD(true);
    }

    private void Disconnect()
    {
        Steph?.Disconnect(this);

        HUD?.Disconnect(this);
        ShowHUD(false);
    }

    public void ShowHUD(bool visible) => HUD?.SetActiveCanvas(visible);
    #endregion
    #endregion
}

[Serializable]
public class MovingSequence
{
    [Tooltip("Dirección a la que se moverá el líder (Steph) al terminar la secuencia.")]
    public Vector2 movingDirection;

    [Tooltip("Tiempo que se moverá el líder (Steph) al terminar la secuencia.")]
    [Min(0f)] public float movingDuration = 0f;

    public SequenceStep danceSequence;
}
