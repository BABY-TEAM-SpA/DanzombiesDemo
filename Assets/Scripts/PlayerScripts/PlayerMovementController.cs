using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private DanceBrain _danceBrain;
    [SerializeField] private bool AllowInput = false;
    [SerializeField] private float speed = 15f;
    [Range(0,15)]public float acceleration;
    private Vector3 direction;
    public bool CanMove = false;
    public Vector3 velocity { private set; get; } = Vector3.zero;
    

    void Start()
    {
        SetSpeed();
    }
    void Update()
    {
        HandleMovement();
    }

    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (AllowInput)
        {
            
            SetDirectionToMove(new Vector3(input.x, input.y, 0));
        }
    }
    public void SetDirectionToMove(Vector3 dir)
    {
        direction = dir;
        _danceBrain.SetBodyDirection(dir.x);
    }
    private void HandleMovement()
    {
        if (CanMove)
        {
            velocity = Vector3.Lerp(velocity, direction * speed, acceleration * Time.deltaTime);
        }
        else
        {
            velocity = Vector3.zero;
        }
        transform.position += velocity * Time.deltaTime;
        _danceBrain.OnMoving(velocity.magnitude);
    }
    
    public void EnableMovement()
    {
        CanMove = true;   
    }

    public void DisableMovement()
    {
        CanMove = false;
    }
    
    public void SetSpeed(float speed=10f)
    {
        speed = speed;
    }
}
