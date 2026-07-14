using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static InteReactableComponent;

public class InteReactableComponent : MonoBehaviour
{
    #region [VARIABLES]
    [SerializeField] private InteReaction[] inteReactions;

    private Dictionary<string, Dictionary<InteReactableArea.Type, InteReaction.InteReactionEvent>> inteReactionsMap = new();
    private InteReactableFeedback feedback;

    [Serializable]
    public class InteReaction
    {
        [Tooltip("Tag del GameObject que aceptará los colliders y eventos configurados en el inspector.")]
        [TagField] public string tag;
        public InteReactionEvent[] events;

        [Serializable]
        public class InteReactionEvent
        {
            public InteReactableArea.Type type;
            public bool didPrimaryTrigger; // <- Si ya se disparó OnPrimary
            public int timesIntended; // <- Veces que el Player ha interactuado/entrado en el área de reacción

            [Tooltip("Asignar GameObject que contiene los colliders que activarán el InteReaction.")]
            public Collider2D area;

            [Tooltip("Veces necesarias para que se ejecute OnPrimary.")]
            [Range(1, 10)] public int timesToPrimary = 1;
            [Tooltip("- Interactable: Al interactuar.\n- Rectable: Al entrar en el área de reacción.")]
            public UnityEvent OnPrimary;

            [Tooltip("Veces necesarias para que se ejecute OnSecondary. Dejar en 0 para ignorar.")]
            [Range(0, 10)] public int timesToSecondary;
            [Tooltip("- Interactable: Al reinteractuar post-interacción exitosa (e.g. cerrar puerta).\n- Reactable: Al abandonar área de reacción.")]
            public UnityEvent OnSecondary;
        }
    }
    #endregion

    #region [UNITY]
    private void Awake()
    {
        foreach (InteReaction inteReaction in inteReactions)
        {
            Dictionary<InteReactableArea.Type, InteReaction.InteReactionEvent> map = new();
            foreach (InteReaction.InteReactionEvent e in inteReaction.events)
            {
                SetupArea(e.area, e.type);
                map[e.type] = e;
            }
            inteReactionsMap[inteReaction.tag.ToString()] = map;
        }

        feedback = GetComponentInChildren<InteReactableFeedback>();
        if (feedback == null)
            Debug.LogWarning($"[{name}] No se encontró un InteReactableFeedback en la jerarquía, " +
                $"el Player podrá interactuar con él, pero no habrá feedback visual.", this);
    }

    private void OnValidate()
    {
        if (inteReactions == null)
            return;

        int maxEvents = Enum.GetValues(typeof(InteReactableArea.Type)).Length;

        foreach (InteReaction inteReaction in inteReactions)
        {
            if (inteReaction?.events == null)
                continue;

            // No más de un InteReactionEvent por InteReactableArea.Type posible
            if (inteReaction.events.Length > maxEvents)
            {
                Debug.LogWarning($"[{name}] InteReaction '{inteReaction.tag}' no puede tener más de {maxEvents} " +
                    $"InteReactionEvent. Se recortó el array.", this);
                Array.Resize(ref inteReaction.events, maxEvents);
            }

            // Chequeo de InteReactableArea.Type repetidos
            HashSet<InteReactableArea.Type> seenTypes = new();
            foreach (InteReaction.InteReactionEvent e in inteReaction.events)
            {
                if (!seenTypes.Add(e.type))
                    Debug.LogWarning($"[{name}] InteReaction '{inteReaction.tag}' tiene más de un " +
                        $"InteReactionEvent con Type '{e.type}'. En el Dictionary solo va a sobrevivir el último.", this);
            }
        }
    }
    #endregion

    #region [METHODS]
    #region API
    public void Interact()
    {
        if (!inteReactionsMap.TryGetValue("Player", out Dictionary<InteReactableArea.Type, InteReaction.InteReactionEvent> map))
            return;
        if (!map.TryGetValue(InteReactableArea.Type.Interactable, out InteReaction.InteReactionEvent e))
            return;

        feedback?.Pulse();
        HandleEventTrigger(e);
    }
    #endregion

    #region Helpers
    private void SetupArea(Collider2D collider, InteReactableArea.Type type)
    {
        if (collider.gameObject == gameObject)
            Debug.LogWarning($"[{name}] El Collider2D '{collider.name}' está en el mismo GameObject. " +
                $"Siempre es preferible que áreas distintas (e.g. Reactable " +
                $"e Interactable) estén en GameObjects independientes.", this);

        if (!collider.TryGetComponent<InteReactableArea>(out InteReactableArea area))
            area = collider.AddComponent<InteReactableArea>();

        area.OnEntered += HandleAreaEntered;
        area.OnExited += HandleAreaExited;
        area.Setup(type);
    }

    private void HandleEventTrigger(InteReaction.InteReactionEvent e)
    {
        e.timesIntended++;
        switch (e.didPrimaryTrigger)
        {
            case true: // OnSecondary
                if (e.timesIntended == e.timesToSecondary)
                {
                    e.OnSecondary?.Invoke();
                    ToggleTimesIntended(e);
                }
                break;

            case false: // OnPrimary
                if (e.timesIntended == e.timesToPrimary)
                {
                    e.OnPrimary?.Invoke();
                    ToggleTimesIntended(e);
                }
                break;
        }
    }

    private void ToggleTimesIntended(InteReaction.InteReactionEvent e)
    {
        e.didPrimaryTrigger = !e.didPrimaryTrigger;
        e.timesIntended = 0;
    }
    #endregion
    #endregion

    #region [EVENTS]
    private void HandleAreaEntered(Collider2D collision, InteReactableArea.Type type)
    {
        if (!inteReactionsMap.TryGetValue(collision.tag, out Dictionary<InteReactableArea.Type, InteReaction.InteReactionEvent> map))
            return;
        if (!map.TryGetValue(type, out InteReaction.InteReactionEvent e))
            return;

        switch (type)
        {
            case InteReactableArea.Type.Interactable:
                if (!collision.TryGetComponent(out PlayerInteractionController player))
                    break;

                feedback?.Show();
                player?.SetInteractive(this);
                break;

            case InteReactableArea.Type.Reactable:
                if (!e.didPrimaryTrigger)
                    HandleEventTrigger(e);
                break;
        }
    }

    private void HandleAreaExited(Collider2D collision, InteReactableArea.Type type)
    {
        if (!inteReactionsMap.TryGetValue(collision.tag, out Dictionary<InteReactableArea.Type, InteReaction.InteReactionEvent> map))
            return;
        if (!map.TryGetValue(type, out InteReaction.InteReactionEvent e))
            return;

        switch (type)
        {
            case InteReactableArea.Type.Interactable:
                if (!collision.TryGetComponent(out PlayerInteractionController player))
                    break;

                feedback?.Hide();
                player?.ClearInteractive();
                break;

            case InteReactableArea.Type.Reactable:
                if (e.didPrimaryTrigger)
                    HandleEventTrigger(e);
                break;
        }
    }
    #endregion
}