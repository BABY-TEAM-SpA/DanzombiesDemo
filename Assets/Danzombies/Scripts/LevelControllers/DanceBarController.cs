using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DanceBarController : MonoBehaviour
{
    public bool isActive = false;

    [Header("Barras de Flow")] [SerializeField]
    private Image IconImage;

    private int currentReaction = 0;
    [SerializeField] private Sprite IconDefaultState;
    [SerializeField] private Sprite[] IconStates = new Sprite[] { };
    [SerializeField] private List<Image> FlowBars = new List<Image>();
    [SerializeField] private Color[] barReactions = new Color[] { };
    [SerializeField] private List<Image> beatBars = new List<Image>();
    [SerializeField] private Material beatBarMaterial;
    [SerializeField] private UiAnimator uiAnimator;
    public UnityEvent onBarFilled;
    public bool isBarFilled { private set; get; } = false;

    public static DanceBarController DanceBar;

    private void Awake()
    {
        if (DanceBar == null)
            DanceBar = this;
        else Destroy(gameObject);
    }
    public void Start()
    {

        Material newMat = new Material(beatBarMaterial);
        beatBarMaterial = newMat;
        foreach (Image bar in beatBars)
        {
            bar.material = newMat;
        }
        //PlayerManager.Player.danceBar = this; // [Frco] Lo cambié para que sea el propio PlayerManager quien busca y asigna la DanceBar
        UpdateFlowBars(0);
        UpdateIconFeedback();
    }

    public void Activate(bool activation)
    {
        isActive = activation;
        UpdateFlowBars(PlayerManager.Player.flow);
        UpdateIconFeedback();
        uiAnimator?.PlaySequence(isActive ? "Open" : "Close");
    }
    
    
    public void UpdateFlowBars(int value)
    {
        if (value == 10)
        {
            if (!isBarFilled)
            {
                isBarFilled = true;
                onBarFilled?.Invoke();
            }
        }
        else
        {
            isBarFilled = false;
            if (value <= 2) currentReaction = 2;
            else if (value <= 5) currentReaction = 1;
            else if (value <= 10) currentReaction = 0;
        }
        
        foreach (Image barra in FlowBars)
        {
            barra.fillAmount = value/10f;
            //barra.color = barReactions[currentReaction];
            //beatBarMaterial.SetFloat("_RainbowEnabled", value==10?1f:0f);
        }
        UpdateIconFeedback();
    }
    public void UpdateIconFeedback()
    {
        if (isActive)
        {
            if(IconImage !=null) IconImage.sprite = IconStates[currentReaction];
        }
        else
        {
            if(IconImage !=null) IconImage.sprite =  IconDefaultState;
            beatBarMaterial.SetFloat("_RainbowEnabled", 0f);
        }
        if(IconImage !=null) IconImage.sprite = isActive ? IconStates[currentReaction] : IconDefaultState;
    }
}
