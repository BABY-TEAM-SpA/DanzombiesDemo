using System;
using UnityEngine;

public class ThrownZombie : MonoBehaviour
{
    #region [VARIAIBLES]
    [SerializeField] Animator animator;

    private AnimatorStateInfo animState;
    private ZombieChasingHordeThrower horde;

    public Action OnPlayerCollided;
    #endregion

    #region [UNITY]
    private void Start() => enabled = false;

    private void Update()
    {
        animState = animator.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName("ThrowItself") && animState.normalizedTime >= 1f)
        {
            transform.SetParent(null, true);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enabled)
            horde?.CatchPlayer();

        if (collision.CompareTag("Zombie") && !enabled)
            horde?.RecoverZombie(this);
    }

    private void OnBecameInvisible() => horde?.RecoverZombie(this);
    #endregion

    #region [METHODS]
    public void Throw(ZombieChasingHordeThrower horde)
    {
        this.horde = horde;
        enabled = true;
    }
    #endregion
}
