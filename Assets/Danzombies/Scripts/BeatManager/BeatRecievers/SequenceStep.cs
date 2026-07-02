using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DanceStep
{
    None,
    L_North,
    R_North,
    L_South,
    R_South,
    L_West,
    R_West,
    L_East,
    R_East
}
[Serializable]
public class SequenceStep
{
    

    [Header("Pattern")]
    public BeatManager.BeatType patternBeatType = BeatManager.BeatType.FullBeat;
    [SerializeField]
    public List<DanceStep> pattern = new();

    public int startCounter { get; set; } = 0;

    
    public enum PlaybackMode{Loop,PlaySequenceXTimes,LoopUntilXCorrectSteps,LoopUntilFlowIsFull}
    public enum SequenceFlowType{FlowAffect_Hurt, FlowAffect_NoHurt,NoFlowAffect_NoHurt}
    [Header("Playback")]
    public PlaybackMode playbackMode = PlaybackMode.Loop;
    public SequenceFlowType sequenceFlowType = SequenceFlowType.FlowAffect_Hurt;
    [Min(1)] public int playerSeqToStop=1;
    [Min(0)] private int SequenceCounter = 0;
    [Min(1)] public int correctDancesToStop = 1;
    private int playerCorrectDancesOnSequence = 0;

    [Header("Events")]
    public UnityEvent OnSequenceCompletedEvent;

    public void ApplyDance(bool isCorrect)
    {
        playerCorrectDancesOnSequence= isCorrect ? playerCorrectDancesOnSequence+1 : 0;
    }
    
    
    public void GetDanceStep(int puzzleCounter , out DanceStep currentStep)
    {
        currentStep = DanceStep.None;
        if (pattern.Count != 0 && puzzleCounter > 0)
        {
            currentStep = pattern[puzzleCounter % pattern.Count];
        };
    }
    
    public Action<RhythmPuzzle> GetNextDanceStep(int puzzleCounter, out DanceStep nextStep) ///largo 4, estoy en el 49 (beat2), y el siguiente es en el 3 (beat4)
    {
        int nextStepCounter = puzzleCounter+1;
        nextStep = DanceStep.None;
        Action<RhythmPuzzle> callback = (RhythmPuzzle puzzle) =>
        {
            puzzle.OnSequenceEnd();
            this.OnSequenceCompletedEvent?.Invoke();
        };
        
        if (pattern.Count != 0 && puzzleCounter > 0)
        {
            switch (playbackMode)
            {
                default:
                    nextStep = CalulateFutureStep(nextStepCounter);
                    return null;
                case PlaybackMode.PlaySequenceXTimes:
                    if ((int)(nextStepCounter-startCounter) / pattern.Count >= playerSeqToStop)
                    {
                        Debug.Log("Ended by Played X Times "+ ((nextStepCounter-startCounter) / pattern.Count).ToString());
                        return callback;
                    }
                    else
                    {
                        nextStep = CalulateFutureStep(nextStepCounter);
                        return null;
                    }
                    break;
                case PlaybackMode.LoopUntilFlowIsFull:
                    if (PlayerManager.Player.flow == 10)
                    {
                        Debug.Log("Ended by Max Flow");
                        return callback;
                    }
                    else
                    {
                        nextStep = CalulateFutureStep(nextStepCounter);
                        return null;
                    }
                case PlaybackMode.LoopUntilXCorrectSteps:
                    if (playerCorrectDancesOnSequence == correctDancesToStop)
                    {
                        Debug.Log("Ended by Dance Correct X Times");
                        return callback;
                    }
                    else
                    {
                        nextStep = CalulateFutureStep(nextStepCounter);
                        if(nextStepCounter%pattern.Count ==0) playerCorrectDancesOnSequence = 0;
                        return null;
                    }
            }
        } 
        return null;
    }

    private DanceStep CalulateFutureStep(int counter)
    {
        for (int i = 0; i < pattern.Count; i++)
        {
            int aux = i+counter;
            aux = aux % pattern.Count;
            if (pattern[aux] != DanceStep.None)
            {
                return pattern[aux];
            }
        }
        return DanceStep.None;
    }
}