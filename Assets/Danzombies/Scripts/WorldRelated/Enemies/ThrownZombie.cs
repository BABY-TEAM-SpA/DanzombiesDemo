using System;
using UnityEngine;

public class ThrownZombie : MonoBehaviour
{
    #region [VARIAIBLES]
    [SerializeField] Animator animator;

    private State state;
    private enum State
    {
        Idle,
        Preparing,
        Throwing,
        Landed
    }

    private ZombieChasingHordeThrower horde;
    private PlayerMovementController player;

    private float speed;
    private Vector3 direction;
    private float delay;
    private float elapsed;

    public Action<ThrownZombie> OnLand;
    #endregion

    #region [UNITY]
    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                return;

            case State.Preparing:
                elapsed += Time.deltaTime;
                if (elapsed >= delay)
                {
                    Throw();
                    elapsed = 0f;
                }
                break;

            case State.Throwing:
                transform.position += speed * direction * Time.deltaTime;
                if (HasLanded())
                    Land();
                break;

            case State.Landed:
                return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !HasLanded())
        {
            Recover();
            horde?.CatchPlayer();
        }
    }

    private void OnBecameInvisible() => Land();
    #endregion

    #region [METHODS]
    public void Prepare(ZombieChasingHordeThrower horde, float speed, PlayerMovementController player, float delay)
    {
        this.horde = horde;
        this.player = player;

        this.speed = speed;
        this.delay = delay;

        state = State.Preparing;
    }

    private void Throw()
    {
        direction = (player.transform.position - transform.position).normalized;

        animator.Play("Throw", 0, 0f);
        state = State.Throwing;
    }

    private void Land()
    {
        speed = 0f;
        direction = Vector3.zero;
        delay = 0f;

        transform.SetParent(null, true);
        state = State.Landed;

        OnLand?.Invoke(this);
    }

    #region Helpers
    private bool HasLanded()
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        return animState.IsName("Throw") && animState.normalizedTime >= 1f;
    }

    public void Recover()
    {
        state = State.Idle;
        elapsed = 0f;

        horde?.RecoverThrownZombie(this);
    }
    #endregion
    #endregion
}
