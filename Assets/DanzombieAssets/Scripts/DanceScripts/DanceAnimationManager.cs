using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PoseAnimationData
{
    public string poseCode;
    //public context;
    public AnimatorOverrideController overrideController;
}

[Serializable]
public class PoseAnimations
{
    public List<PoseAnimationData> animations =new List<PoseAnimationData>();
}

public class DanceAnimationManager : MonoBehaviour
{
    
    [SerializeField] private PoseAnimations allposesData = new PoseAnimations();
    public static DanceAnimationManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
    }

    public PoseAnimationData GetPoseAnimationData(string poseCodeToGet) //, TSource contextToGet)
    {
        PoseAnimationData poseAnimationData = allposesData.animations.FirstOrDefault<PoseAnimationData>(x =>  x.poseCode == poseCodeToGet);// && x.context == contextToGet);
        return poseAnimationData;
    }
}
