using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class PlayerRendererController : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private UiAnimator uiAnimator;
    #endregion

    #region [METHODS]
    public void Blink(bool receiveDamage) =>
        uiAnimator.PlaySequence(receiveDamage ? "Damage" : "Heal");
    #endregion
}
