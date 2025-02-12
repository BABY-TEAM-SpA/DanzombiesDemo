using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReciever : MonoBehaviour
{

    [SerializeField] DanceBrain brain;
    private bool inputClockActive = false;
    [SerializeField] private float inputMargen;
    private float inputClock=0f;

    private string inputDanceCode;
    private bool dancing;
    private bool Linput;
    private string Lmano;
    private string Lpie;
    private string LStickUD;
    private string LStickLR;
    private bool Rinput;
    private string Rmano;
    private string Rpie;
    private string RStickUD;
    private string RStickLR;


    public bool recivedInput = false;
    //public Queue<string> inputRegist = new Queue<string>();
    [SerializeField] InputAction IA;

    public BeatReciever BeatRecieverEnemyzone;


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
            ResetInputs();
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

    public void OnLeftArmTrigger(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            Linput = true;
            inputClockActive = true;
            Lmano = "L1";
        }
        if (callback.canceled)
        {
            SendRelease();
        }
        
    }
    public void OnLeftFeetTrigger(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            Linput = true;
            inputClockActive = true;
            Lpie = "L2";
        }
        if (callback.canceled)
        {
            SendRelease();
        }
    }

    public void GetLeftUD(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            float value = callback.ReadValue<float>();
            if(value > 0f)
            {
                LStickUD = "U";
            }
            else
            {
                LStickUD = "D";
            }
            if (callback.canceled)
            {
                LStickUD = "_";
            }
        }
    }
    public void GetLeftLR(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            float value = callback.ReadValue<float>();
            if (value > 0f)
            {
                LStickLR = "R";
            }
            else
            {
                LStickLR = "L";
            }
        }
        if (callback.canceled)
        {
            LStickLR = "_";
        }
    }

    public void OnRightArmTrigger(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            Rinput = true;
            inputClockActive = true;
            Rmano = "R1";
        }
        if (callback.canceled)
        {
            SendRelease();
        }
    }
    public void OnRightFeetTrigger(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            Rinput = true;
            inputClockActive = true;
            Rpie = "R2";
        }
        if (callback.canceled)
        {
            SendRelease();
        }
    }

    public void GetRightUD(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            float value = callback.ReadValue<float>();
            if (value > 0f)
            {
                RStickUD = "U";
            }
            else
            {
                RStickUD = "D";
            }
        }
        if (callback.canceled)
        {
            RStickUD = "_";
        }
    }
    public void GetRightLR(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            float value = callback.ReadValue<float>();
            if (value > 0f)
            {
                RStickLR = "R";
            }
            else
            {
                RStickLR = "L";
            }
        }
        if (callback.canceled)
        {
            RStickLR = "_";
        }
    }


    private void MakeDanceMove()
    {
        dancing = true;
        Debug.Log("Start Dancing...");
        inputDanceCode = "";
        if (Linput)
        {
            inputDanceCode += Lmano + Lpie + LStickUD + LStickLR;
        }
        if (Rinput)
        {
            inputDanceCode += Rmano + Rpie + RStickUD + RStickLR;
        }
        SendDanceMove();
        
    }

    private void ResetInputs()
    {
        Debug.Log("----Inputs Reseted----");
        Lmano = string.Empty;
        Lpie = string.Empty;
        LStickUD = "x";
        LStickLR = "x";
        Rmano = string.Empty;
        Rpie = string.Empty;
        RStickUD = "x";
        RStickLR = "x";
        inputDanceCode = string.Empty;
        Linput = false;
        Rinput = false;
    }

    public void SendDanceMove()
    {
        //BeatRecieverEnemyzone.SendDance(,inputDanceCode) /// le envia a ALGO el baile hecho para validarlo.
        Debug.Log(inputDanceCode);
        brain.MakeDance(inputDanceCode);

    }

    public void SendRelease()
    {
        if (dancing)
        {
            //Send To STOP Dancing
            //BeatRecieverEnemyzone.SendDance(inputDanceCode);
            dancing = false;
            Debug.Log("...Dancing End.");
            ResetInputs();
        }
    }

    public (bool,string) CheckDanceStatus()
    {
        return (dancing,inputDanceCode);
    }

}
