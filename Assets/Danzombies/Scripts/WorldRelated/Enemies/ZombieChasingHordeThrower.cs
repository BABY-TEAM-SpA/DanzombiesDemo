using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZombieChasingHordeThrower : ObjectPool<ThrownZombie>
{
    #region [VARIABLES]
    [SerializeField] private Transform zombieSpawn;
    [SerializeField] private Image cautionImg;

    [Header("References")]
    [SerializeField] private PlayerMovementController playerMovement;

    [Header("Settings")]
    [Tooltip("Factor al que el zombie saldrá disparado con respecto al Player (2f = x2 de la velocidad de Greg).")]
    [SerializeField][Range(0f, 3f)] private float throwFactor;
    [Tooltip("Tiempo que el zombie pasará 'volando' hasta volver a la pool.")]
    [SerializeField][Range(0f, 5f)] private float throwDuration;
    [SerializeField][Range(1f, 10f)] float throwPeriod;
    [SerializeField][Range(0f, 2f)] float throwDelay;

    private PlayerManager player;
    private float elapsed;
    private float throwSpeed;

    private Coroutine throwRoutine;
    private Coroutine blinkRoutine;
    #endregion

    #region [UNITY]
    private void Start()
    {
        if (playerMovement != null)
        {
            player = playerMovement.GetComponent<PlayerManager>();
            throwSpeed = playerMovement.MaxSpeed * throwFactor;
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
    #endregion

    #region [COROUTINES]
    private IEnumerator ThrowZombieRoutine()
    {
        float t = 0f;
        ThrownZombie thrownZombie = Get();
        thrownZombie.transform.SetParent(null, true);

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkCautionRoutine());

        yield return new WaitForSeconds(throwDelay); // <- Delay
        
        thrownZombie.Enable(this);
        Vector3 direction = (playerMovement.transform.position - thrownZombie.transform.position).normalized;
        
        while (t < throwDuration)
        {
            thrownZombie.transform.localPosition += direction * throwSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

        thrownZombie.Disable();
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
