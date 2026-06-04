using UnityEngine;

public class GenericFeedbackElement : FeedbackElement
{
    [SerializeField] private GameObject element;
    public override void Activate(bool isActive)
    {
        element.SetActive(isActive);
    }
}
