using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    #region [VARIABLES]
    [SerializeField] protected T prefab;
    [SerializeField][Range(1, 100)] protected int poolSize = 3;

    private Queue<T> pool = new();
    #endregion

    #region [METHODS]
    /// <summary>
    /// Prepara todas las instancias de la pool necesarias. Se les puede asignar un nuevo padre
    /// que seguirá siendo vigente para los métodos Get() y Return().
    /// </summary>
    public virtual List<T> Prewarm(Transform parent = null)
    {
        Clear();
        List<T> instances = new List<T>();

        for (int i = 0; i < poolSize; i++)
        {
            T instance = Create(parent);
            pool.Enqueue(instance);
            instances.Add(instance);
        }

        return instances;
    }

    public virtual T Create(Transform parent = null)
    {
        T instance = Instantiate(prefab, parent ?? transform);
        instance.gameObject.SetActive(false);
        return instance;
    }

    /// <summary>
    /// Obtiene la primera instancia en la cola y, opcionalmente, le asigna un nuevo padre.
    /// Generalmente se querrá usar para dejar la instancia en una UnitBattle o BattlePlate.
    /// </summary>
    public virtual T Get(Transform parent = null)
    {
        T instance = pool.Count > 0
            ? pool.Dequeue()
            : Create();

        Transform prewarmedParent = instance.transform.parent?.transform;
        instance.transform.SetParent(parent ?? prewarmedParent ?? transform, false);
        instance.gameObject.SetActive(true);
        OnGet(instance);

        return instance;
    }

    /// <summary>
    /// Retorna la instancia al final de la cola y, opcionalmente, le asigna un nuevo padre.
    /// Generalmnte se querrá usar para devolver la instancia en el container de la pool.
    /// </summary>
    public virtual void Recover(T instance, bool deactivate = true, Transform parent = null)
    {
        Transform prewarmedParent = instance.transform.parent?.transform;
        instance.transform.SetParent(parent ?? prewarmedParent ?? transform, false);
        instance.gameObject.SetActive(!deactivate);
        OnReturn(instance);

        pool.Enqueue(instance);
    }

    /// <summary>
    /// Limpia la pool, destruyendo todas las instancias que contiene y vaciando la cola.
    /// </summary>
    public virtual void Clear()
    {
        while (pool.Count > 0)
        {
            T instance = pool.Dequeue();
            if (instance != null)
                Destroy(instance.gameObject);
        }
    }
    #endregion

    #region [HOOKS]
    protected virtual void OnGet(T instance) { }
    protected virtual void OnReturn(T instance) { }
    #endregion
}