using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    [SerializeField] Canvas loadingCanvas;
    private bool loading = false;
    
    [Serializable]
    public class LoadScenePack
    {
        public List<string> scenes = new List<string>();
        public bool shouldStopMusic=false;
    }
    public static SceneChangeController Instance { get; private set; }
    
    private LoadScenePack scenesToLoad;
    public bool isBusy = false;

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

    public Coroutine ExecuteLoadPlan()
    {
        if(isBusy) return null;
        isBusy = true;
        return StartCoroutine(ChangeSceneCoroutine(scenesToLoad));
        
    }

    public IEnumerator ChangeSceneCoroutine(LoadScenePack scenesToLoad)
    {
        for (int i = 0; i < scenesToLoad.scenes.Count; i++)
        {
            yield return LoadAdditive(scenesToLoad.scenes[i]);
        }
        scenesToLoad = null;
        isBusy = false;
        loadingCanvas.gameObject.SetActive(false);
    }

    private IEnumerator LoadAdditive(string sceneName)
    {
        //AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
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

    private void LoadInterScene()
    {
        loadingCanvas.gameObject.SetActive(false);
        ExecuteLoadPlan();
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
