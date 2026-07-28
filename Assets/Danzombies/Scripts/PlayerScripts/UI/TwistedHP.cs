using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra de vida sobre la cabeza de Greg.
/// </summary>
public class TwistedHP : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private GameObject root;
    [SerializeField] private UiAnimator uiAnimator;
    [SerializeField] private Image[] hearths;
    #endregion

    #region [UNITY]
    private void Awake() => root.gameObject.SetActive(false);
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
        uiAnimator.PlaySequence("Show");
    }
    #endregion
}
