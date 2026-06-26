using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] Canvas loadingCanvas;

    public static SceneChangeController Instance { get; private set; }

    private LoadScenePack scenesPack;
    private Coroutine loadingCoroutine;
    private Coroutine unloadingCoroutine;

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

    #region [UNITY]
    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else Instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region [METHODS]
    #region API
    public void LoadScenes(LoadScenePack scenesPack)
    {
        this.scenesPack = scenesPack;
        if (scenesPack.shouldStopMusic)
            AudioManager.Instance.StopSong(); // <- [Frco] FMOD Update
        LoadInterScene();
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
    private void LoadInterScene()
    {
        switch (scenesPack.chargeMode)
        {
            case ChargeSceneMode.Sync:
                ForceLoadScene(scenesPack.scenes[0]);
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
        AudioManager.Instance.StopSong(); // <- [Frco] FMOD Update
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    #endregion
    #endregion

    #region [COROUTINES]
    #region Load
    private IEnumerator LoadAsyncRoutine(LoadScenePack scenesPack)
    {
        loadingCanvas.gameObject.SetActive(scenesPack.loadMode != LoadSceneMode.Additive);

        foreach (string sceneName in scenesPack.scenes)
            yield return LoadAsyncRoutine(sceneName, scenesPack.loadMode);

        this.scenesPack = null;
        loadingCoroutine = null;
        loadingCanvas.gameObject.SetActive(false);
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
