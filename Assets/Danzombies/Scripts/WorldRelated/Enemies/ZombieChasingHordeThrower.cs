using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZombieChasingHordeThrower : ObjectPool<PlayerCollisionDetector>
{
    #region [VARIABLES]
    [SerializeField] private Transform zombieSpawn;
    [SerializeField] private Image cautionImg;

    [Header("References")]
    [SerializeField] private PlayerMovementController player;

    [Header("Settings")]
    [SerializeField] bool active;
    [Tooltip("Factor al que el zombie saldrá disparado con respecto al Player (2f = x2 de la velocidad de Greg).")]
    [SerializeField][Range(0f, 3f)] private float throwFactor;
    [Tooltip("Tiempo que el zombie pasará 'volando' hasta volver a la pool.")]
    [SerializeField][Range(0f, 5f)] private float throwDuration;
    [SerializeField][Range(1f, 10f)] float throwPeriod;
    [SerializeField][Range(0f, 2f)] float throwDelay;

    private float elapsed;
    private float throwSpeed;
    private Coroutine throwRoutine;
    private PlayerCollisionDetector thrownZombie;
    #endregion

    #region [UNITY]
    private void Start()
    {
        if (player != null)
            throwSpeed = player.MaxSpeed * throwFactor;
        cautionImg.gameObject.SetActive(false);

        Prewarm(zombieSpawn);
    }

    private void Update()
    {
        if (!active)
            return;
        if (throwRoutine != null) // <- Para que el ThrowDelay no entre en el contador del ThrowPeriod
            return;

        elapsed += Time.deltaTime;
        if (elapsed >= throwPeriod)
        {
            ThrowZombie();
            elapsed = 0f;
        }
    }

    private void OnBecameVisible() => enabled = true;
    private void OnBecameInvisible() => enabled = false;
    #endregion

    #region [METHODS]
    #region API
    public void Activate() => active = true;
    public void Deactivate() => active = false;
    #endregion

    #region Behaviour
    private void ThrowZombie()
    {
        if (throwRoutine != null)
            StopCoroutine(throwRoutine);
        throwRoutine = StartCoroutine(ThrowZombieRoutine());
    }

    private void CatchPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        RecoverZombie();
    }
    //FindFirstObjectByType<CheckpointsManager>().RecoverToLastCeckpoint();
    #endregion

    #region Helpers
    private void RecoverZombie()
    {
        thrownZombie.OnPlayerCollided -= CatchPlayer;
        thrownZombie.transform.localPosition = Vector3.zero;
        Recover(thrownZombie, true, zombieSpawn);

        if (throwRoutine != null)
            StopCoroutine(throwRoutine);
        throwRoutine = null;
    }
    #endregion
    #endregion

    #region [COROUTINES]
    private IEnumerator ThrowZombieRoutine()
    {
        float t = 0f;
        thrownZombie = Get();
        thrownZombie.transform.SetParent(null, true);
        thrownZombie.OnPlayerCollided += CatchPlayer;

        cautionImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(throwDelay); // <- Delay
        cautionImg.gameObject.SetActive(false);

        Vector3 direction = (player.transform.position - thrownZombie.transform.position).normalized;
        while (t < throwDuration)
        {
            thrownZombie.transform.localPosition += direction * throwSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

        RecoverZombie(); // <- Caso opuesto a CatchPlayer(): el zombie pasó de largo
    }

    private IEnumerator BlinkCaution()
    {
        yield return null;
    }
    #endregion
}
