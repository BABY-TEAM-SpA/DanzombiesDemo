using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Componente que se acopla en un GameObject para que reaccione cuando el Player pase cerca de él.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ReactableComponent : MonoBehaviour
{
    #region [VARIABLES]
    [Tooltip("Conectar con el método que se ejecutará cuando el Player pase cerca de este GameObject.")]
    public UnityEvent OnReact;
    [Tooltip("Conectar con el método que se ejecutará cuando el Player se aleje de este GameObject.")]
    public UnityEvent OnDismiss;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    #region Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled && collision.CompareTag("Player"))
            OnReact?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enabled && collision.CompareTag("Player"))
            OnDismiss?.Invoke();
    }
    #endregion
    #endregion

    #region [METHODS]
    #region Dis/Enable
    public void Enable() => enabled = true;
    public void Disable() => enabled = false;
    #endregion
    #endregion
}
