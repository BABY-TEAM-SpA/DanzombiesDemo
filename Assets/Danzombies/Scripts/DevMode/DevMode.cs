using UnityEngine;
using UnityEngine.InputSystem;

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

    public static DevMode Instance {  get; private set; } // <- Singleton
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if (Instance != null && Instance == this)
            Destroy(gameObject);
        else Instance = this;

        root = transform.GetChild(0);
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

    private void PlayFrom()
    {
        Debug.Log($"Play From");
    }
    #endregion
}
