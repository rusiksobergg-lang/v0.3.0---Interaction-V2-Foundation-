using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour
{
    public List<InteractionAction> actions =
        new List<InteractionAction>();

    public virtual void Interact(InteractionType actionType)
    {
        Debug.Log(
            $"{gameObject.name}: {actionType}");
    }
}