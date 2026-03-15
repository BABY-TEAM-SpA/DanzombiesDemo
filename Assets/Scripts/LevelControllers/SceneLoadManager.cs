using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    [SerializeField] Canvas loadingCanvas;
    private bool loading = false;

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
        public bool shouldStopMusic=false;
    }
    public static SceneChangeController Instance { get; private set; }
    
    private LoadScenePack scenesToLoad;
    public bool isBusy = false;
    private Coroutine loadingCoroutine;

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        } 
    }

    public void LoadScenes(LoadScenePack scenesPack)
    {   
        scenesToLoad = scenesPack;
        if(scenesPack.shouldStopMusic) AudioManager.Instance.StopSong();
        LoadInterScene();
    }
    
    private void LoadInterScene()
    {
        bool sync = scenesToLoad.chargeMode == ChargeSceneMode.Sync;
        if (!sync)
        {
            loadingCanvas.gameObject.SetActive(scenesToLoad.loadMode != LoadSceneMode.Additive);
            loadingCoroutine = ExecuteLoadAsyncPlan();
        }
        else
        {
            ForceLoadScene(scenesToLoad.scenes[0]);
        }
        
    }
    
    public Coroutine ExecuteLoadAsyncPlan()
    {
        if(isBusy) return null;
        isBusy = true;
        return StartCoroutine(LoadAsyncCoroutine(scenesToLoad));
        
    }

    public IEnumerator LoadAsyncCoroutine(LoadScenePack scenesToLoad)
    {
        for (int i = 0; i < scenesToLoad.scenes.Count; i++)
        {
            yield return LoadAsync(scenesToLoad.scenes[i], scenesToLoad.loadMode);
        }
        scenesToLoad = null;
        isBusy = false;
        loadingCanvas.gameObject.SetActive(false);
    }

    private IEnumerator LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        //AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, mode);
        if(loadOp == null) yield break;
        loadOp.allowSceneActivation = false;
        while (loadOp.progress < 0.9f)
        {
            yield return null;
        }
        loadOp.allowSceneActivation = true;
        while (!loadOp.isDone)
        {
            yield return null;
        }
        
    }

    

    public void ForceLoadScene(string sceneName)
    {
        AudioManager.Instance.StopSong();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
