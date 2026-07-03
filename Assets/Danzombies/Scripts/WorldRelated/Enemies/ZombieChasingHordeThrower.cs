using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZombieChasingHordeThrower : ObjectPool<ThrownZombie>, IResettable
{
    #region [VARIABLES]
    [SerializeField] private Image cautionImg;

    [Header("References")]
    [SerializeField] private PlayerMovementController playerMovement;

    [Header("Settings")]
    [SerializeField][Range(1f, 10f)] float throwPeriod;
    [SerializeField][Range(0f, 2f)] float throwDelay;

    private PlayerManager player;
    private Transform zombieSpawn;
    private float elapsed;

    private Coroutine throwRoutine;
    private Coroutine blinkRoutine;
    #endregion

    #region [UNITY]
    private void Awake() => zombieSpawn = transform.Find("ZombieSpawn").GetComponent<Transform>();

    private void Start()
    {
        if (playerMovement != null)
        {
            player = playerMovement.GetComponent<PlayerManager>();
            //throwSpeed = playerMovement.MaxSpeed * throwFactor;
        }

        Prewarm(zombieSpawn);
    }

    private void Update()
    {
        if (throwRoutine != null) // <- Para que el ThrowDelay no entre en el contador del ThrowPeriod
            return;

        elapsed += Time.deltaTime;
        if (elapsed >= throwPeriod)
        {
            ThrowZombie();
            elapsed -= throwPeriod;
        }
    }

    private void OnBecameVisible() => enabled = true;
    private void OnBecameInvisible() => enabled = false;
    #endregion

    #region [METHODS]
    #region API
    public void Enable() => enabled = true;
    public void Disable() => enabled = false;
    #endregion

    #region Behaviour
    private void ThrowZombie()
    {
        if (throwRoutine != null)
            StopCoroutine(throwRoutine);
        throwRoutine = StartCoroutine(ThrowZombieRoutine());
    }

    public void CatchPlayer() => player.GameOver();
    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //FindFirstObjectByType<CheckpointsManager>().RecoverToLastCeckpoint();

    public void RecoverZombie(ThrownZombie thrownZombie)
    {
        thrownZombie.transform.localPosition = Vector3.zero;
        Recover(thrownZombie, true, zombieSpawn);
    }
    #endregion

    #region IResettable
    public void CaptureState() { }

    public void ResetState()
    {
        if (throwRoutine != null)
            StopCoroutine(throwRoutine);
        throwRoutine = null;

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = null;
    }
    #endregion
    #endregion

    #region [COROUTINES]
    private IEnumerator ThrowZombieRoutine()
    {
        ThrownZombie thrownZombie = Get();

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkCautionRoutine());

        yield return new WaitForSeconds(throwDelay); // <- Delay

        thrownZombie.Throw(this);
        //Vector3 direction = (playerMovement.transform.position - thrownZombie.transform.position).normalized;
        //Vector3 vDirection = Vector3.Dot(direction, Vector3.up) * Vector3.up;

        yield return new WaitWhile(() => thrownZombie.enabled); // <- Animación del ThrownZombie
        throwRoutine = null;
    }

    private IEnumerator BlinkCautionRoutine()
    {
        cautionImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(throwDelay); // <- Delay
        cautionImg.gameObject.SetActive(false);

        blinkRoutine = null;
    }
    #endregion
}
