using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DanceBrain _danceBrain; //TODO: separate this from the zombiebrains WHY DO THE ZOMBIE BRAINS USE PLAYERMOVEMENTCONTROLLER HELP ME

    [Header("Params")]
    [SerializeField] private bool AllowInput = false;
    [SerializeField] private float Speed = 15f;
    [Range(0,50)]public float acceleration;

    [Header("Variables")]
    private Vector3 direction;
    public Vector3 velocity { private set; get; } = Vector3.zero;
    float currentSpeedFactor = 1.0f;
    int boostBeats = -1;

    void Start()
    {
        SetSpeed();
    }

    public void setDanceBrain(DanceBrain toSet) //function to be called by playerManager
    {
        _danceBrain = toSet;
    }
    
    void Update()
    {
        if(_danceBrain.isActive && AllowInput) HandleMovement();
        else
        {
            velocity = Vector3.zero;
        }
    }
    
    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        SetDirectionToMove(new Vector3(input.x, input.y, 0));
    }
    
    public void SetDirectionToMove(Vector3 dir)
    {
        direction = dir;
    }
    
    private void HandleMovement()
    {
        velocity = Vector3.Lerp(velocity, direction * Speed * currentSpeedFactor, acceleration * Time.deltaTime);
        if(velocity.magnitude < 0.1f)
        {
            velocity = Vector3.zero;
        }
        transform.position += velocity * Time.deltaTime;
        _danceBrain.OnMoving(direction);
        _danceBrain.SetBodyDirection(direction.x);
    }
    
    public void EnableMovement()
    {
        AllowInput = true;
    }

    public void DisableMovement()
    {
        AllowInput = false;
        direction = Vector3.zero;
        velocity = Vector3.zero;
    }
    
    public void SetSpeed(float speed=10f)
    {
        Speed = speed;
    }

    public void MultiplySpeedFor4Beats(float multiplier = 2.0f)
    {
        currentSpeedFactor = multiplier;
        boostBeats = 4;
    }

    public void PassBeat() //called by playerBeatReceiver whenever a beat passes
    {
        if (boostBeats < 0) return;
        boostBeats -= 1;
        if (boostBeats <= 0)
        {
            currentSpeedFactor = 1.0f;
            boostBeats = -1;
        }
    }
}