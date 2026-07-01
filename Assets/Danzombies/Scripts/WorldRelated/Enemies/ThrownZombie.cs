using System;
using UnityEngine;

public class ThrownZombie : MonoBehaviour
{
    #region [VARIAIBLES]
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float timeBeforeJumping;

    private float elapsed;
    private ZombieChasingHordeThrower horde;

    public Action OnPlayerCollided;
    #endregion

    #region [UNITY]
    private void Start() => enabled = false;

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= timeBeforeJumping)
        {
            animator.SetTrigger("Jump");
            elapsed = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            horde?.CatchPlayer();

        if (collision.CompareTag("Zombie") && !enabled)
            horde?.RecoverZombie(this);
    }

    private void OnBecameInvisible() => horde?.RecoverZombie(this);
    #endregion

    #region [METHODS]
    public void Enable(ZombieChasingHordeThrower horde)
    {
        this.horde = horde;
        enabled = true;
    }

    public void Disable() => enabled = false;
    #endregion
}
