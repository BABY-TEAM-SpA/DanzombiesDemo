using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZombieChasingHordeThrower : ObjectPool<ThrownZombie>, IResettable
{
    #region [VARIABLES]
    [SerializeField] private Image cautionImg;

    [Header("References")]
    [SerializeField] private PlayerMovementController playerMovement;

    [Header("Settings")]
    [SerializeField][Range(10f, 30f)] float throwSpeed;
    [SerializeField][Range(1f, 10f)] float throwPeriod;
    [Tooltip("Variación en el periodo.")]
    [SerializeField][Range(0f, 2f)] float throwDelta;
    [Tooltip("Retardo en el lanzamiento (cuanto tiempo está avisando que viene un zombie).")]
    [SerializeField][Range(0f, 2f)] float throwDelay;

    private PlayerManager player;
    private Transform zombieSpawn;
    private ThrownZombie throwingZombie;
    private float elapsed;
    private float period;

    private Coroutine blinkRoutine;
    #endregion

    #region [UNITY]
    private void Awake()
    {
        if (playerMovement != null)
            player = playerMovement.GetComponent<PlayerManager>();
        zombieSpawn = transform.Find("ZombieSpawn").GetComponent<Transform>();

        Prewarm(zombieSpawn);
        SetPeriod();
    }

    private void Update()
    {
        if (throwingZombie != null)
            return;

        elapsed += Time.deltaTime;
        if (elapsed >= period)
        {
            ThrowZombie();
            SetPeriod();
            elapsed -= period;
        }
    }
    #endregion

    #region [METHODS]
    #region Behaviour
    private void ThrowZombie()
    {
        throwingZombie = Get(zombieSpawn);
        throwingZombie.OnLand += OnThrowingZombieLanded;

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkCautionRoutine());

        throwingZombie.Prepare(this, throwSpeed, playerMovement, throwDelay);
    }

    public void CatchPlayer()
    {
        throwingZombie?.Recover();
        throwingZombie = null;
        player?.GameOver();
    }

    public void RecoverThrownZombie(ThrownZombie thrownZombie) => Recover(thrownZombie, true, zombieSpawn);
    #endregion

    #region IResettable
    public void CaptureState() { }

    public void ResetState()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        cautionImg.gameObject.SetActive(false);
        blinkRoutine = null;
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

    #region [COROUTINES]
    private IEnumerator BlinkCautionRoutine()
    {
        cautionImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(throwDelay); // <- Delay
        cautionImg.gameObject.SetActive(false);

        blinkRoutine = null;
    }
    #endregion

    #region [EVENTS]
    private void OnThrowingZombieLanded(ThrownZombie throwZombie)
    {
        if (throwZombie == throwingZombie)
        {
            throwingZombie.OnLand -= OnThrowingZombieLanded;
            throwingZombie = null;
        }
    }
    #endregion
}
