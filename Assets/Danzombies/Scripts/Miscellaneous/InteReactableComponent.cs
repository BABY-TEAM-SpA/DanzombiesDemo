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
    private InteReactableFeedback feedback;
    private int timesInteracted;

    [Serializable]
    public class InteReaction
    {
        [Tooltip("Tag del GameObject que aceptará los colliders y eventos configurados en el inspector.")]
        [TagField] public string tag;

        [Tooltip("Flag para marcar que ya se interactuó con el GameObject. Cuando sea true, OnInteract deja de considerarse y OnReinteract será quien escuche si el Player interactúa.")]
        public bool interacted;

        public InteReactionArea[] areas;
        [Serializable]
        public class InteReactionArea
        {
            public InteReactableArea.Type type;
            public Collider2D[] colliders;
        }

        [Tooltip("Conectar con el método que se ejecutará cuando el Player pase cerca de este GameObject.")]
        public UnityEvent OnReact;

        [Range(1, 10)] public int timesToInteract;
        [Tooltip("Conectar con el método que se ejecutará cuando el Player interactúe con este GameObject.")]
        public UnityEvent OnInteract;

        [Range(0, 10)] public int timesToReinteract;
        [Tooltip("Conectar con el método que se ejecutará cuando el Player reinteractúe con este GameObject después de haber interactuado exitosamente con él. Colocar 0 para ignorar esta opción.")]
        public UnityEvent OnReinteract;

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

        feedback = GetComponentInChildren<InteReactableFeedback>();
        if (feedback == null)
            Debug.LogWarning($"El InteReactableComponent '{name}' no encontró un InteReactableFeedback en la jerarquía," +
                $"el Player podrá interactuar con él, pero no habrá feedback visual.", this);
    }
    #endregion

    #region [METHODS]
    #region API
    public void Interact()
    {
        if (!inteReactionsMap.TryGetValue("Player", out InteReaction inteReaction))
            return;

        feedback?.Pulse();
        timesInteracted++;

        switch (inteReaction.interacted)
        {
            case true: // OnReinteract
                if (timesInteracted == inteReaction.timesToReinteract)
                    inteReaction.OnReinteract?.Invoke();
                break;

            case false: // OnInteract
                if (timesInteracted == inteReaction.timesToInteract)
                {
                    inteReaction.OnInteract?.Invoke();
                    inteReaction.interacted = true;
                    timesInteracted = 0;
                }
                break;
        }
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

                feedback?.Show();
                player?.SetInteractive(this);
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

                feedback?.Hide();
                player?.ClearInteractive();
                break;
        }
    }
    #endregion
}