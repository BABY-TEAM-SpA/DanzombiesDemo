using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevRespawn : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private TextMeshProUGUI ambientTmp;
    [SerializeField] private TextMeshProUGUI respawnTmp;
    [SerializeField] private Button playButton;
    #endregion

    #region [METHODS]
    #endregion
    public void Setup(string sceneName, string respawnId, Action PlayFrom)
    {
        ambientTmp.text = sceneName;
        respawnTmp.text = respawnId;

        playButton.onClick.AddListener(() => PlayFrom());
    }
}
