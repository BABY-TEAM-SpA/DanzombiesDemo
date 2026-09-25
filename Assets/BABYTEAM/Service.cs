using UnityEngine;

/// <summary>
/// Expone una instancia global para que cualquier script pueda llamar al <Service>.Instance.<Method>().
/// </summary>
public abstract class Service<T> : MonoBehaviour where T : Service<T>
{
    public static T Instance {  get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"[{typeof(T).Name}] Duplicado en '{name}'.", this);
            Destroy(this);
            return;
        }

        Instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
