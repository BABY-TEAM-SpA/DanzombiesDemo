using System;
using UnityEngine;

public class ThrownZombie : MonoBehaviour
{
    #region [VARIAIBLES]
    [SerializeField,Range(20f,0f)] private float minimunDistance;
    [SerializeField] Animator animator;
    [SerializeField]private Transform target;

    [SerializeField] private State state;
    private enum State
    {
        Idle,
        Running,
        Jumping,
        Landing
    }
    
    [SerializeField] private float speed=5f;
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
                transform.position += distance.normalized * (speed * Time.deltaTime);
                if (distance.magnitude<=minimunDistance)
                    Jump();
                break;
            case State.Jumping:
                transform.position += Vector3.right *(speed*0.8f * Time.deltaTime);
                break;
            case State.Landing:
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
                player.GameOver();
            }
        }
        else
        {
            if(other.TryGetComponent(out ZombieChasingHordeThrower thrower))
            {
                thrower.ResetState();
                Destroy(this.gameObject);
            }
        }
        
    }
    #endregion

    #region [METHODS]
    public void Throw(ZombieChasingHordeThrower horde, float speedValue, Transform player)
    {
        target = player;
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
