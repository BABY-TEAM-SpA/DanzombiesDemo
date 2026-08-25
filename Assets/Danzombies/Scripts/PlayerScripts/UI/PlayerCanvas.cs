using UnityEngine;

/// <summary>
/// Canvas de UI sobre Greg.
/// </summary>
public class PlayerCanvas : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private PlayerManager player;

    [Header("Elements")]
    public TwistedHP TwistedHP;
    #endregion

    #region [UNITY]
    private void OnEnable()
    {
        if (player != null)
            player.OnPlayerDeath += ResetHP;
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnPlayerDeath -= ResetHP;
    }
    #endregion

    #region [METHODS]
    #region TwistedHP
    public void SetHP(int value) => TwistedHP?.SetHP(value);
    public void UpdateHP() => TwistedHP?.SetHP(player.HP);
    public void ResetHP() => TwistedHP?.Reset();
    #endregion
    #endregion
}
