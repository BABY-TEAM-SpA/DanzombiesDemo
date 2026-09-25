using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChangeController : Service<SceneChangeController>
{
    #region [VARIABLES]
    private LoadScenePack scenesPack;
    private Coroutine loadingCoroutine;
    private Coroutine unloadingCoroutine;

    public UnityEvent OnLoadStarted;
    public UnityEvent OnLoadCompleted;

    #region Structures
    public enum ChargeSceneMode
    {
        Sync,
        Async
    }

    [Serializable]
    public class LoadScenePack
    {
        public ChargeSceneMode chargeMode = ChargeSceneMode.Sync;
        public LoadSceneMode loadMode;
        public List<string> scenes = new List<string>();
        public bool shouldStopMusic;
    }

    [Serializable]
    public class UnloadScenePack
    {
        public List<string> scenes = new List<string>();
    }
    #endregion
    #endregion

    #region [METHODS]
    #region API
    public void LoadScenes(LoadScenePack scenesPack)
    {
        OnLoadStarted?.Invoke();

        this.scenesPack = scenesPack;
        if (scenesPack.shouldStopMusic)
            AudioManager.Instance.StopSong();
    }

    public void UnloadScenes(UnloadScenePack scenesPack)
    {
        if (scenesPack?.scenes == null || scenesPack.scenes.Count == 0)
            return;
        if (unloadingCoroutine != null)
            return;

        unloadingCoroutine = StartCoroutine(UnloadAsyncRoutine(scenesPack));
    }
    #endregion

    #region Helpers
    public void LoadInterScene()
    {
        switch (scenesPack.chargeMode)
        {
            case ChargeSceneMode.Sync:
                //ForceLoadScene(scenesPack.scenes[0]); <- [Frco] La transición de escenas queda más smooth si ocupamos la corrutina async
                LoadAsync();
                break;

            case ChargeSceneMode.Async:
                LoadAsync();
                break;
        } 
    }
    
    private void LoadAsync()
    {
        if (loadingCoroutine != null)
            return;
        loadingCoroutine = StartCoroutine(LoadAsyncRoutine(scenesPack));
        
    }

    private void ForceLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        OnLoadCompleted?.Invoke();
    }
    #endregion
    #endregion

    #region [COROUTINES]
    #region Load
    private IEnumerator LoadAsyncRoutine(LoadScenePack scenesPack)
    {
        foreach (string sceneName in scenesPack.scenes)
            yield return LoadAsyncRoutine(sceneName, scenesPack.loadMode);

        this.scenesPack = null;
        loadingCoroutine = null;
        OnLoadCompleted?.Invoke();
    }

    private IEnumerator LoadAsyncRoutine(string sceneName, LoadSceneMode mode)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, mode);
        if (loadOp == null)
            yield break;

        loadOp.allowSceneActivation = false;
        while (loadOp.progress < 0.9f)
            yield return null;
        
        loadOp.allowSceneActivation = true;
        while (!loadOp.isDone)
            yield return null;
    }
    #endregion

    #region Unload
    private IEnumerator UnloadAsyncRoutine(UnloadScenePack pack)
    {
        foreach (string sceneName in pack.scenes)
            yield return UnloadAsyncRoutine(sceneName);

        unloadingCoroutine = null;
    }

    private IEnumerator UnloadAsyncRoutine(string sceneName)
    {
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
        if (unloadOp == null)
            yield break;

        while (!unloadOp.isDone)
            yield return null;
    }
    #endregion
    #endregion
}
