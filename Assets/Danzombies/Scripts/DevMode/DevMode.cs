using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DevMode : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private CheckpointsCatalog catalog;
    [SerializeField] private DevRespawn devRespawnPrefab;
    [SerializeField] private InputActionReference devModeRef;

    [Header("Components")]
    [SerializeField] private Transform content;

    private Transform root;
    private bool isShowing;

    public static DevMode Instance { get; private set; } // <- Singleton
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        root = transform.GetChild(0);
        HideCanvas();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
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
        ClearCanvas();
        FillCanvas();
        root.gameObject.SetActive(true);
        isShowing = true;
    }

    private void HideCanvas()
    {
        root.gameObject.SetActive(false);
        ClearCanvas();
        isShowing = false;
    }

    private void FillCanvas()
    {
        foreach (CheckpointsCatalog.SceneRespawns respawns in catalog.Respawns)
            foreach (string respawn in respawns.respawns)
            {
                DevRespawn devRespawn = Instantiate(devRespawnPrefab, content, false);
                devRespawn.Setup(respawns.sceneName, respawn, PlayFrom);
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
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
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
