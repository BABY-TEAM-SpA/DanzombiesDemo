using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public enum StickPose
{
    xx,
    Ux,
    UL,
    UR,
    xL,
    xR,
    DL,
    DR,
    Dx,
}
[Serializable]
public class DancePose
{
    [Header("Left Side")]
    [SerializeField] public bool LeftHand;
    [SerializeField] public bool LeftFoot;
    [SerializeField] public StickPose LeftStick;

    [Header("Right Side")]
    [SerializeField] public bool RightHand;
    [SerializeField] public bool RightFoot;
    [SerializeField] public StickPose RightStick;

    [Header("Time To Wait in Compases")]
    public int waitTimes = 0;
    public int poseDuration = 1;

    public string GetDanceCode()
    {
        string poseCode = "";
        if (LeftHand || LeftFoot)
        {
            poseCode += (LeftHand ? "L1":"")+(LeftFoot ? "L2":"")+LeftStick.ToString();
        }
        if (RightHand || RightFoot)
        {
            poseCode += (RightHand ? "R1":"")+(RightFoot ? "R2":"")+RightStick.ToString();
        }
        return poseCode;
    }
}

[CreateAssetMenu(fileName = "new Choreography", menuName = "Dancezombies/Choreography")]
public class ScriptableChoreography : ScriptableObject
{
    public List<DancePose> choreography;
}
