using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class InteReactableComponent : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InteReaction[] inteReactions;

    private Dictionary<string, InteReaction> inteReactionsMap = new();

    [Serializable]
    public class InteReaction
    {
        [Tooltip("Tag del GameObject que aceptará los colliders y eventos configurados en el inspector.")]
        [TagField] public string tag;

        public InteReactionArea[] areas;
        [Serializable]
        public class InteReactionArea
        {
            public InteReactableArea.Type type;
            public Collider2D[] colliders;
        }

        [Tooltip("Conectar con el método que se ejecutará cuando el Player pase cerca de este GameObject.")]
        public UnityEvent OnReact;
        [Tooltip("Conectar con el método que se ejecutará cuando el Player interactúe con este GameObject.")]
        public UnityEvent OnInteract;
        [Tooltip("Conectar con el método que se ejecutará cuando el Player se aleje de este GameObject.")]
        public UnityEvent OnLeave;
    }
    #endregion

    #region [UNITY]
    private void Awake()
    {
        foreach (InteReaction inteReaction in inteReactions)
        {
            foreach (InteReaction.InteReactionArea area in inteReaction.areas)
                SetupAreas(area.colliders, area.type);
            inteReactionsMap[inteReaction.tag.ToString()] = inteReaction;
        }
    }
    #endregion

    #region [METHODS]
    #region API
    public void Interact()
    {
        if (!inteReactionsMap.TryGetValue("Player", out InteReaction inteReaction))
            return;

        //feedback?.Pulse();
        inteReaction.OnInteract?.Invoke();
    }
    #endregion

    #region Helpers
    private void SetupAreas(Collider2D[] colliders, InteReactableArea.Type type)
    {
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject)
                Debug.LogWarning($"El Collider2D '{collider.name}' está en el mismo GameObject que" +
                    $"InteReactableComponent. Siempre es preferible que áreas distintas (e.g. Reactable" +
                    $"e Interactable) estén en GameObjects independientes.", this);

            if (!collider.TryGetComponent<InteReactableArea>(out InteReactableArea area))
                area = collider.AddComponent<InteReactableArea>();

            area.OnEntered += HandleAreaEntered;
            area.OnExited += HandleAreaExited;
            area.Setup(type);
        }
    }
    #endregion
    #endregion

    #region [EVENTS]
    private void HandleAreaEntered(Collider2D collision, InteReactableArea.Type type)
    {
        if (!inteReactionsMap.TryGetValue(collision.tag, out InteReaction inteReaction))
            return;

        switch (type)
        {
            case InteReactableArea.Type.Reactable:
                inteReaction.OnReact?.Invoke();
                break;

            case InteReactableArea.Type.Interactable:
                if (!collision.TryGetComponent(out PlayerInteractionController player))
                    break;
                player?.SetInteractive(this);
                Debug.Log($"Setting Interactive");
                break;
        }
    }

    private void HandleAreaExited(Collider2D collision, InteReactableArea.Type type)
    {
        if (!inteReactionsMap.TryGetValue(collision.tag, out InteReaction inteReaction))
            return;

        switch (type)
        {
            case InteReactableArea.Type.Reactable:
                inteReaction.OnLeave?.Invoke();
                break;

            case InteReactableArea.Type.Interactable:
                if (!collision.TryGetComponent(out PlayerInteractionController player))
                    break;
                player?.ClearInteractive();
                break;
        }
    }
    #endregion
}