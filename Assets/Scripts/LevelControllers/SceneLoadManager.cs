using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    [Serializable]
    public class LoadScenePack
    {
        public List<string> scenes = new List<string>();
        public bool shouldStopMusic=false;
    }
    public static SceneChangeController Instance { get; private set; }
    
    private LoadScenePack scenesToLoad;

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } 
    }

    public void LoadScenes(LoadScenePack scenesPack)
    {   
        scenesToLoad = scenesPack;
        if(scenesPack.shouldStopMusic) AudioManager.Instance.StopSong();
        LoadInterScene();
    }

    public void LoadSceneAdditive(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void ActivateQueueScenesLoad()
    {
        for (int i = 0; i < scenesToLoad.scenes.Count; i++)
        {
            SceneManager.LoadSceneAsync(scenesToLoad.scenes[i], (i==0)?LoadSceneMode.Single:LoadSceneMode.Additive);
        }
        scenesToLoad = null;
    }

    private void LoadInterScene()
    {
        SceneManager.LoadScene("LoadingScreen");
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
