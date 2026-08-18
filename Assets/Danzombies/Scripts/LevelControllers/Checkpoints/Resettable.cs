using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Resettable : MonoBehaviour
{
    #region [VARIABLES]
    public ResetConfig[] states;
    
    [Serializable]
    public class ResetConfig
    {
        public string name;
        public Checkpoint respawn;
        public UnityEvent OnReset;
    }
    #endregion

    #region [UNITY]
    #endregion

    #region [METHODS]
    public void ResetState(Checkpoint checkpoint)
    {
        ResetConfig config = states.FirstOrDefault(r => r.respawn == checkpoint);
        config?.OnReset?.Invoke();
    }
    #endregion
}
