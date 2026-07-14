using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteReactableArea : MonoBehaviour
{
    #region [VARIABLES]
    private Type type;
    public enum Type
    {
        Reactable,
        Interactable,
    }

    public Action<Collider2D, Type> OnEntered;
    public Action<Collider2D, Type> OnExited;
    #endregion

    #region [UNITY]
    private void Awake() => gameObject.SetActive(false);

    #region Trigger
    private void OnTriggerEnter2D(Collider2D collision) => OnEntered?.Invoke(collision, type);
    private void OnTriggerExit2D(Collider2D collision) => OnExited?.Invoke(collision, type);
    #endregion
    #endregion

    #region [METHODS]
    public void Setup(Type type)
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.isTrigger = true;

        if (type == Type.Interactable)
            gameObject.layer = 7; // <- Interactable

        this.type = type;
        gameObject.SetActive(true);
    }
    #endregion
}
