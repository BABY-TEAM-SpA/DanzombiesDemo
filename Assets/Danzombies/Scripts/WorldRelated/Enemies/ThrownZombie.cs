using System;
using UnityEngine;

public class ThrownZombie : MonoBehaviour
{
    #region [VARIAIBLES]
    [SerializeField][Range(0f, 20f)] private float minimunDistance;
    [SerializeField] Animator animator;
    [SerializeField] private Transform target;

    [SerializeField] private State state;
    private enum State
    {
        Idle,
        Running,
        Jumping,
        Landing
    }
    private ZombieChasingHordeThrower thrower;
    
    [SerializeField] private float speed = 5f;
    #endregion

    #region [UNITY]
    private void Update()
    {
        Vector3 distance = target.position - transform.position;
        switch (state)
        {
            case State.Idle:
                return;

            case State.Running:
                distance.y *= 3f;
                transform.position += distance.normalized * (speed * Time.deltaTime);
                if (distance.magnitude <= minimunDistance)
                    Jump();
                break;

            case State.Jumping:
                transform.position += Vector3.right * (speed * 0.8f * Time.deltaTime);
                break;

            case State.Landing:
                if (thrower.transform.position.x >= transform.position.x)
                    Destroy(gameObject);
                return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state != State.Landing)
        {
            if (other.TryGetComponent(out PlayerManager player))
            {
                //Recover();
                player.GetLifeDamage();
            }
        }
        else
        {
            if (other.TryGetComponent(out ZombieChasingHordeThrower thrower))
            {
                //thrower.ResetState();
                Destroy(gameObject);
            }
        }
    }
    #endregion

    #region [METHODS]
    public void Throw(ZombieChasingHordeThrower horde, float speedValue, Transform player)
    {
        target = player;
        thrower = horde;
        speed = speedValue;
        state = State.Running;
    }

    private void Jump()
    {
        animator.Play("Jump");
        state = State.Jumping;
    }

    private void Land()
    {
        speed = 0f;
        transform.SetParent(null, true);
        state = State.Landing;
    }
    #endregion
}
