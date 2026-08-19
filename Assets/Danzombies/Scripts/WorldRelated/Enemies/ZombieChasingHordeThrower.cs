using UnityEngine;

public class ZombieChasingHordeThrower : ObjectPool<ThrownZombie>
{
    #region [VARIABLES]
    private bool canThrow;
    [SerializeField] private Transform zombieSpawn;
    [SerializeField] private UiAnimator warningElement;

    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] bool makeFirstZombieMiss;

    [Header("Settings - Numbers")]
    [SerializeField][Range(10f, 45f)] float throwSpeed = 30f;
    [SerializeField][Range(1f, 10f)] float throwPeriod;

    [Tooltip("Variación en el periodo.")]
    [SerializeField][Range(0f, 2f)] float throwDelta;
    
    private ThrownZombie throwingZombieInstance;

    private float elapsed;
    private float period;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        Prewarm(zombieSpawn);
        SetPeriod();
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
    public void SetThrow(bool value)
    {
        if (throwingZombieInstance != null)
            Destroy(throwingZombieInstance.gameObject);
        canThrow = value;
    }

    private void StartThrowAction() => warningElement.PlaySequence(0);

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

    #region Helpers
    private void SetPeriod()
    {
        float delta = throwDelta * 0.5f;
        float rng = Random.Range(-delta, delta);
        period = throwPeriod + rng;
    }

    public void SetFirstZombieMiss(bool value) => makeFirstZombieMiss = value;
    #endregion
    #endregion
}
