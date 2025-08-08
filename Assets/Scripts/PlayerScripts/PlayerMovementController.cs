using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private bool CanMove = false;
    public bool isWalking = false;
    [SerializeField] private float moveSpeed = 0f;
    private float currentSpeed = 0f;
    private Vector3 direction;
    public Vector3 velocity { private set; get; } = Vector3.zero;
    private Animator movementAnimator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        if (CanMove)
        {
            Vector2 input = context.ReadValue<Vector2>();
            Move(new Vector3(input.x, input.y, 0));
        }
    }

    public void Move(Vector3 dir)
    {
        if(dir!= Vector3.zero) isWalking=true;
        else{ isWalking = false; }
        direction = dir;
    }

    public void SetSpeed(float speed=-1f)
    {
        if (speed == -1f)
        {
            currentSpeed = moveSpeed;
        }
        else
        {
            currentSpeed = speed;
        }
    }
    private void HandleMovement()
    {
        if (isWalking)
        {  
            velocity = Vector3.Lerp(velocity, direction * currentSpeed, currentSpeed);
            if(movementAnimator!=null) movementAnimator.SetBool("LookingLeft",velocity.x<0);
        }
        else {
            velocity = Vector3.zero;
        }
        transform.position += velocity * Time.deltaTime;
    }

    

    public void EnableMovement()
    {
        CanMove = true;   
    }

    public void DisableMovement()
    {
        CanMove = false;
    }
}
