using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPuzzle : RhythmPuzzle
{
    #region [VARIABLES]
    [Header("Moving Settings")]
    [SerializeField] private ZombieDanceBrain Steph;
    [SerializeField] private TutorialDanceBrain HUD;
    public MovingSequence[] movingSequences;
    [Serializable]
    public class MovingSequence
    {
        [Tooltip("Dirección a la que se moverá el líder (Steph) al terminar la secuencia.")]
        public Vector2 movingDirection;

        [Tooltip("Tiempo que se moverá el líder (Steph) al terminar la secuencia.")]
        [Min(0f)] public float movingDuration = 0f;
    }

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

    public override void SetActivePuzzle(bool activate)
    {
        base.SetActivePuzzle(activate);
        ShowHUD(activate);

        if (!activate)
            return;

        isMoving = false;
        SetWholeSequence();
    }
    #endregion

    #region RhythmPuzzle - Update
    public override void OnPuzzleCompleted()
    {
        index++;
        SetWholeSequence();
    }
    #endregion

    #region RythmPuzzle - Player
    //public bool SetPlayerInput(DanceStep playerStep, out BeatFeedback bf)
    //{
    //    bool danced = false;//base.SetPlayerInput(playerStep, out bf);
    //    bool isCorrect = playerStep == currentDanceData.DanceStep;

    //    if (danced && isCorrect && !isMoving)
    //        StartMoving();
    //    return danced;
    //}
    #endregion

    #region BeatReceiver
    public override void PreBeatAction(int beat, BeatManager.BeatType type)
    {
        if (debug)
            Debug.Log("___Puzzle PreBeat on " + AudioManager.Instance.SongPositionSeconds().ToString());

        currentStep = danceSequence.GetDanceStep(beat, type);
        eventManager.InvokePrepare(beat, type, currentStep);
    }

    public override void BeatAction(int beat, BeatManager.BeatType type)
    {
        eventManager.InvokeDance(beat, type, currentStep);
    }

    public override void PostBeatAction(int beat, BeatManager.BeatType type)
    {
        if (debug)
            Debug.Log("___Puzzle PostBeat on " + AudioManager.Instance.SongPositionSeconds().ToString());

        eventManager.InvokeRealease(beat, type, currentStep);
    }
    #endregion

    #region Helpers
    private void SetWholeSequence()
    {
        if (index < movingSequences.Length)
            lastSequence = movingSequences[index];
        else SetActivePuzzle(false);
    }

    private void Connect()
    {
        eventManager.AddListener(Steph);
        eventManager.AddListener(HUD);
        ShowHUD(true);
    }

    private void Disconnect()
    {
        eventManager.RemoveListener(Steph);
        eventManager.RemoveListener(HUD);
        ShowHUD(false);
    }

    public void ShowHUD(bool visible) => HUD?.SetActiveCanvas(visible);
    #endregion
    #endregion
}