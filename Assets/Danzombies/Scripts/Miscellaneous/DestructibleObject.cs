using UnityEngine;
using UnityEngine.Events;

public class DestructibleObject : MonoBehaviour
{
    public UnityEvent OnDestruction;
    [SerializeField] Animator animator;
    
    
    public void Destroy()
    {
        OnDestruction?.Invoke();
        animator?.Play("Complete");
    }
}
