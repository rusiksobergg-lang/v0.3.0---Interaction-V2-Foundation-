using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public Camera playerCamera;
    public float interactionDistance = 2f;
    public InteractionHUD interactionHUD;

    private int currentActionIndex = 0;
    private Interactable currentInteractable;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (interactionHUD != null)
            interactionHUD.Hide();
    }

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f)
        );

        if (Physics.SphereCast(
            ray,
            0.12f,
            out RaycastHit hit,
            interactionDistance))
        {
            Interactable interactable =
                hit.collider.GetComponentInParent<Interactable>();

            if (interactable != null)
            {
                HandleInteractable(interactable);
                return;
            }
        }

        ClearInteraction();
    }

    private void HandleInteractable(Interactable interactable)
    {
        // Якщо навелися на новий об'єкт
        if (interactable != currentInteractable)
        {
            currentInteractable = interactable;
            currentActionIndex = 0;
        }

        List<InteractionAction> availableActions =
            interactable.GetAvailableActions();

        if (availableActions.Count == 0)
        {
            ClearInteraction();
            return;
        }

        // Захист індексу
        if (currentActionIndex >= availableActions.Count)
            currentActionIndex = 0;

        HandleActionSelection(availableActions);

        InteractionAction currentAction =
            availableActions[currentActionIndex];

        string objectName =
            interactable.itemData != null
            ? interactable.itemData.displayName
            : interactable.gameObject.name;

        string itemInfo = GetItemInfo(interactable);

        string actionText =
            $"[F] {currentAction.displayName}";

        // Оновлюємо HUD
        interactionHUD.Show(
            objectName,
            itemInfo,
            actionText
        );

        // Натискання F
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            interactable.Interact(
                currentAction.type,
                transform
            );
        }
    }

    private void HandleActionSelection(
        List<InteractionAction> availableActions)
    {
        if (Mouse.current == null)
            return;

        float scroll =
            Mouse.current.scroll.ReadValue().y;

        if (scroll > 0.1f)
        {
            currentActionIndex--;

            if (currentActionIndex < 0)
                currentActionIndex =
                    availableActions.Count - 1;
        }
        else if (scroll < -0.1f)
        {
            currentActionIndex++;

            if (currentActionIndex >= availableActions.Count)
                currentActionIndex = 0;
        }
    }

    private string GetItemInfo(Interactable interactable)
    {
        WorldItem worldItem =
            interactable.GetComponent<WorldItem>();

        if (worldItem == null)
            return "";

        ItemInstance instance =
            worldItem.GetItemInstance();

        if (instance == null ||
            instance.itemData == null)
            return "";

        string condition =
            GetConditionLabel(instance);

        return
            $"{condition} • " +
            $"{instance.itemData.weight:0.0} кг";
    }

    private string GetConditionLabel(ItemInstance instance)
    {
        if (instance.itemData == null)
            return "";

        float percent =
            (float)instance.currentCondition /
            instance.itemData.maxCondition * 100f;

        if (percent >= 90)
            return "<color=#57D65B>Pristine</color>";

        if (percent >= 70)
            return "<color=#B8E04A>Worn</color>";

        if (percent >= 45)
            return "<color=#FFB347>Damaged</color>";

        if (percent >= 15)
            return "<color=#FF6B4A>Badly Damaged</color>";

        return "<color=#8A8A8A>Ruined</color>";
    }

    private void ClearInteraction()
    {
        if (currentInteractable != null)
        {
            currentInteractable = null;
            currentActionIndex = 0;
        }

        if (interactionHUD != null)
            interactionHUD.Hide();
    }
}