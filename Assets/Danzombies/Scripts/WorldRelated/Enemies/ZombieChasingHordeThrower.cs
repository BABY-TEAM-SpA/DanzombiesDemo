using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZombieChasingHordeThrower : ObjectPool<ThrownZombie>, IResettable
{
    #region [VARIABLES]
    private bool canThrow = false;
    [SerializeField] private UiAnimator warningElement;

    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] bool makeFirstZombieMiss;

    [Header("Settings - Numbers")]
    [SerializeField][Range(10f, 45f)] float throwSpeed = 30f;
    [SerializeField][Range(1f, 10f)] float throwPeriod;
    [Tooltip("Variación en el periodo.")]
    [SerializeField][Range(0f, 2f)] float throwDelta;
    
    private Transform zombieSpawn; 
    ThrownZombie throwingZombieInstance;
    private float elapsed;
    private float period;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        zombieSpawn = transform.Find("ZombieSpawn").GetComponent<Transform>();

        Prewarm(zombieSpawn);
        SetPeriod();
    }

    public void ActivateThrown(bool value)
    {
        canThrow = value;
    }

    private void Update()
    {
        if (!canThrow)
            return;
        if (throwingZombieInstance != null)
            return;

        elapsed += Time.deltaTime;
        if (elapsed >= period)
        {
            StartThrowAction();
            SetPeriod();
            elapsed -= period;
        }
    }
    #endregion

    #region [METHODS]
    #region Behaviour
    private void StartThrowAction()
    {
        warningElement.PlaySequence(0);
    }

    public void ThrowZombie()
    {
        throwingZombieInstance = Get(zombieSpawn);
        throwingZombieInstance.transform.SetParent(null, true);

        if (makeFirstZombieMiss)
        {
            throwingZombieInstance.minimunDistance *= 2f;
            makeFirstZombieMiss = false;
        }

        throwingZombieInstance.Throw(this, throwSpeed, target);
    }
    #endregion

    #region IResettable
    public void CaptureState() { }

    public void ResetState()
    {
        
    }

    #endregion

    #region Helpers
    private void SetPeriod()
    {
        float delta = throwDelta * 0.5f;
        float rng = Random.Range(-delta, delta);
        period = throwPeriod + rng;
    }
    #endregion
    #endregion
}
