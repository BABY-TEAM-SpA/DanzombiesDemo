using UnityEngine;

public class ZombieChasingHordeThrower : ObjectPool<ThrownZombie>, IResettable
{
    #region [VARIABLES]
    private bool canThrow = false;
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
    private ZombieChasingHordeThrowerState _state;

    private float elapsed;
    private float period;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        Prewarm(zombieSpawn);
        SetPeriod();
    }

    public void ActivateThrown(bool value) => canThrow = value;

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

    #region Helpers
    private void SetPeriod()
    {
        float delta = throwDelta * 0.5f;
        float rng = Random.Range(-delta, delta);
        period = throwPeriod + rng;
    }
    #endregion
    #endregion

    #region IResettable
    private struct ZombieChasingHordeThrowerState
    {
        public bool canThrow;
        public bool makeFirstZombieMiss;

        public float throwSpeed;
        public float throwPeriod;
        public float throwDelta;
    }

    public void CaptureState()
    {
        _state = new ZombieChasingHordeThrowerState
        {
            canThrow = canThrow,
            makeFirstZombieMiss = makeFirstZombieMiss,

            throwSpeed = throwSpeed,
            throwPeriod = throwPeriod,
            throwDelta = throwDelta
        };
    }

    public void ResetState()
    {
        canThrow = _state.canThrow;
        makeFirstZombieMiss = _state.makeFirstZombieMiss;

        throwSpeed = _state.throwSpeed;
        throwPeriod = _state.throwPeriod;
        throwDelta = _state.throwDelta;

        elapsed = 0f;

        if (throwingZombieInstance != null)
            Destroy(throwingZombieInstance.gameObject);
    }
    #endregion
}
