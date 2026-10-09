using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevMode : Service<DevMode>
{
    #region [VARIABLES]
    [SerializeField] private CheckpointsCatalog catalog;
    [SerializeField] private DevRespawn devRespawnPrefab;
    [SerializeField] private InputActionReference devModeRef;

    [Header("Components")]
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private Transform content;

    private Transform root;
    private Transform aux; // <- [Frco] Temporal, hasta que el menú de pausa exista y tenga las opciones de sonido
    private bool isShowing;
    private Dictionary<string, LoadScenePack> scenePacks = new(); // [SceneName] -> LoadScenePack
    #endregion

    #region [UNITY]
    private void Start()
    {
        root = transform.GetChild(0);
        aux = transform.GetChild(1);
        HideCanvas();

        ClearCanvas();
        FillCanvas();
    }

    private void LateUpdate()
    {
        if (devModeRef.action.WasPressedThisFrame())
        {
            if (isShowing)
                HideCanvas();
            else ShowCanvas();
        }
    }
    #endregion

    #region [METHODS]
    #region Canvas
    private void ShowCanvas()
    {
        root.gameObject.SetActive(true);
        aux.gameObject.SetActive(true);
        raycaster.enabled = true;
        isShowing = true;
    }

    private void HideCanvas()
    {
        root.gameObject.SetActive(false);
        aux.gameObject.SetActive(false);
        raycaster.enabled = false;
        isShowing = false;
    }

    private void FillCanvas()
    {
        foreach (CheckpointsCatalog.SceneRespawns respawns in catalog.Respawns)
            foreach (string respawn in respawns.respawns)
            {
                string sceneName = respawns.sceneName;

                DevRespawn devRespawn = Instantiate(devRespawnPrefab, content, false);
                devRespawn.Setup(sceneName, respawn, PlayFrom);

                LoadScenePack scenePack = new LoadScenePack(sceneName, true);
                scenePacks[sceneName] = scenePack;
            }
    }

    private void ClearCanvas()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);
    }
    #endregion

    #region Respawn
    private void PlayFrom(string sceneName, string respawn)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == sceneName)
        {
            RespawnInScene(currentScene, respawn);
            return;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (scene.name == sceneName)
                RespawnInScene(scene, respawn);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneChangeController.Instance.LoadScenes(scenePacks[sceneName]);
    }

    private void RespawnInScene(Scene scene, string respawn)
    {
        CheckpointsManager manager = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            manager = root.GetComponentInChildren<CheckpointsManager>(true);
            if (manager != null)
                break;
        }

        if (manager == null)
        {
            Debug.LogError($"[DevMode] No se encontró un CheckpointsManager en la escena '{scene.name}'.");
            return;
        }

        if (!manager.TryGetCheckpointByName(respawn, out Checkpoint checkpoint))
        {
            Debug.LogError($"[DevMode] El respawn '{respawn}' ya no existe en '{scene.name}'." +
                $"Vuelve a apretar el botón Collect Resettables & Update Catalog del CheckpointsManager en la escena.");
            return;
        }

        PlayerManager player = FindAnyObjectByType<PlayerManager>();
        if (player == null)
        {
            Debug.LogError($"[DevMode] No se encontró un PlayerManager en la escena '{scene.name}'.");
            return;
        }

        manager.RecoverTo(checkpoint, player);
        Debug.Log($"[DevMode] Salto a Checkpoint '{respawn}' en '{scene.name}'.");
    }
    #endregion
    #endregion
}
