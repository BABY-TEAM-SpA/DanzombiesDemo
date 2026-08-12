using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "CheckpointsCatalog", menuName = "Scriptable Objects/CheckpointsCatalog")]
public class CheckpointsCatalog : ScriptableObject
{
    #region Variables
    public IReadOnlyCollection<SceneRespawns> Respawns => respawnsByScene;
    [SerializeField] private SceneRespawns[] respawnsByScene = Array.Empty<SceneRespawns>();

    /// <summary>
    /// Estructura alternativa a un Dictionary<string, int[]>, donde la Key es el nombre de la escena y el Value
    /// el índice del Checkpoint IsRespawn bajo el CheckpointsManager de su respectiva escena.
    /// </summary>
    [Serializable] public struct SceneRespawns
    {
        public string sceneName;
        public string[] respawns;
    }
    #endregion

    #region Methods
#if UNITY_EDITOR
    public void SetRespawns(string sceneName, string[] respawns)
    {
        SceneRespawns newEntry = new SceneRespawns
        {
            sceneName = sceneName,
            respawns = respawns
        };

        int index = Array.FindIndex(respawnsByScene, entry => entry.sceneName == sceneName);
        if (index >= 0)
        {
            respawnsByScene[index] = newEntry;
            return;
        }

        Array.Resize(ref respawnsByScene, respawnsByScene.Length + 1);
        respawnsByScene[^1] = newEntry;
    }
#endif

    public bool TryGetRespawnIds(string sceneName, out string[] respawns)
    {
        int index = Array.FindIndex(respawnsByScene, entry => entry.sceneName == sceneName);
        if (index < 0)
        {
            respawns = Array.Empty<string>();
            return false;
        }

        respawns = respawnsByScene[index].respawns;
        return true;
    }
    #endregion
}
