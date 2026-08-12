using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CheckpointsCatalog;

public class DevModeWindow : EditorWindow
{
    #region [VARIABLES]
    private CheckpointsCatalog catalog;
    #endregion

    #region [UNITY]
    private void OnEnable()
    {
        catalog = AssetDatabase.LoadAssetAtPath<CheckpointsCatalog>(
            "Assets/Danzombies/Scripts/DevMode/CheckpointsCatalog.asset");

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

    private void OnGUI()
    {
        if (catalog == null)
        {
            EditorGUILayout.HelpBox($"No se encontró el CheckpointsCatalog.", MessageType.Error);
            return;
        }

        FillWindow();
    }
    #endregion

    #region [METHODS]
    #region Window
    [MenuItem("Window/DevMode &#d")]
    public static void ShowWindow()
    {
        GetWindow<DevModeWindow>("DevMode");
    }

    private void FillWindow()
    {
        foreach (CheckpointsCatalog.SceneRespawns respawns in catalog.Respawns)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label($"{respawns.sceneName}", EditorStyles.boldLabel);

            foreach (string respawn in respawns.respawns)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{respawn}");

                GUI.enabled = EditorApplication.isPlaying;
                if (GUILayout.Button("Play", GUILayout.Width(80)))
                    PlayFrom(respawns.sceneName, respawn);
                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
    #endregion

    #region Play Mode
    private void PlayFrom(string sceneName, string respawn)
    {
        Scene crrentScene = SceneManager.GetActiveScene();
        if (crrentScene.name == sceneName)
        {
            RespawnInScene(crrentScene, respawn);
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

        player.transform.position = checkpoint.Spawn;
        Debug.Log($"[DevMode] Salto a Checkpoint '{respawn}' en '{scene.name}'.");
    }
    #endregion
    #endregion

    #region [EVENTS]
    private void OnPlayModeStateChanged(PlayModeStateChange state) => Repaint();
    #endregion
}
