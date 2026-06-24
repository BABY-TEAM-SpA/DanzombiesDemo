using UnityEngine;

public class PersonInWagonBehaviour : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private SpriteRenderer visuals;

    [Header("Customization")]
    public Sprite reactionSprite;

    private Sprite idleSprite;
    #endregion

    #region [UNITY]
    private void Awake() => idleSprite = visuals?.sprite;
    #endregion

    #region [METHODS]
    public void ReactToPlayer()
    {
        Debug.Log($"{name} está reaccionando al Player");
        visuals.sprite = reactionSprite;
    }

    public void DismissPlayer()
    {
        Debug.Log($"El Player se está alejando de {name}");
        visuals.sprite = idleSprite;
    }
    #endregion
}
