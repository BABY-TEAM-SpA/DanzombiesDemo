using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra de vida sobre la cabeza de Greg.
/// </summary>
public class TwistedHP : MonoBehaviour
{
    #region [VARIABLES]
    private const float ACTIVE_TIME = 2.5f;

    [SerializeField] private GameObject root;
    [SerializeField] private Image[] hearths;

    private float elapsed;
    #endregion

    #region [UNITY]
    private void LateUpdate()
    {
        if (!root.gameObject.activeSelf)
            return;

        elapsed += Time.deltaTime;
        if (elapsed > ACTIVE_TIME)
            TurnOff();
    }
    #endregion

    #region [METHODS]
    /// <summary>
    /// Des/activa los corazones de la barra en función de la vida restante.
    /// </summary>
    /// <param name="hp">Vida actual entre 0 y 3.</param>
    public void SetHP(int hp)
    {
        for (int i = 0; i < hearths.Length; i++)
            hearths[i].gameObject.SetActive(i < hp);
        TurnOn();
    }

    public void TurnOn()
    {
        elapsed = 0f;
        root.gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        elapsed = 0f;
        root.gameObject.SetActive(false);
    }
    #endregion
}
