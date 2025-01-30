using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReciever : BeatReciever
{


    public bool reciveInput = false;
    private bool inputClockActive = false;
    [SerializeField] private float inputMargen;
    private float inputClock=0f;
    public Queue<string> inputRegist = new Queue<string>();
    [SerializeField] InputAction IA;

    public static PlayerInputReciever Instance { get; private set; }

    /// <summary>
    /// El player recibe los controles por eje separado en el cuerpo.
    /// Existe un inputMargen el cual es la ventana en que se aceptaran botones antes de mandar la señal de moverse.
    ///     Se tomara el ultimo pressed del axis para ser enviado.
    /// 
    /// Una vez cumplido el margen se toma 
    /// </summary>


    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void Update()
    {
        if(inputClockActive)
        {
            if(inputClock >= inputMargen)
            {
                MakeDanceMove();
                inputClock = 0f;
                inputClockActive =false;
            }
            else
            {
                inputClock += Time.deltaTime; 
            }
        }        
    }

    public override void PreBeatAction()
    {
        inputRegist.Clear();
        reciveInput = true;
    }
    public override void BeatAction()
    {
        //
    }

    public override void PostBeatAction()
    {
        //
        inputRegist.Clear();
        reciveInput = false;
    }


    public void OnLeftArmTrigger(InputAction.CallbackContext callback)
    {
        Debug.Log(callback.ToString());
    }
    public void OnLeftFeetTrigger(InputAction.CallbackContext callback)
    {
        Debug.Log(callback.ToString());
    }

    public void GetLeftMove(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            Debug.Log(callback.ToString());
        }
   
    }

    public void OnRightArmTrigger(InputAction.CallbackContext callback)
    {
        Debug.Log(callback.ToString());
    }
    public void OnRightFeetTrigger(InputAction.CallbackContext callback)
    {
        Debug.Log(callback.ToString());
    }

    public void GetRightMove(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            Debug.Log(callback.ToString());
        }
    }


    private void MakeDanceMove()
    {

    }

    public void SendDanceMove()
    {
        ///
    }

}
