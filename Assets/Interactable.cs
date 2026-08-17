
using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    protected List<InteractionAction> actions =
        new List<InteractionAction>();

    public virtual List<InteractionAction> GetAvailableActions()
    {
        return actions;
    }

    public virtual void Interact(InteractionType actionType, Transform interactor)
    {
        Debug.Log($"{gameObject.name}: {actionType}");
    }
}