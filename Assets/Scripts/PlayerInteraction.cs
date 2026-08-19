
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public Camera playerCamera;
    public TextMeshProUGUI interactionTextUI;
    public float interactionDistance = 2f;

    private int currentActionIndex = 0;
    private Interactable currentInteractable;
    private float leftArrowFlash;
    private float rightArrowFlash;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.SphereCast(ray, 0.12f, out RaycastHit hit, interactionDistance))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();

            if (interactable != currentInteractable)
            {
                currentInteractable = interactable;
                currentActionIndex = 0;
            }

            if (interactable != null)
            {
                List<InteractionAction> availableActions = interactable.GetAvailableActions();

                if (currentActionIndex >= availableActions.Count)
                    currentActionIndex = 0;

                if (availableActions.Count > 0)
                {
                    float scroll = Mouse.current.scroll.ReadValue().y;

                    if (scroll > 0.1f)
                    {
                        currentActionIndex--;

                        if (currentActionIndex < 0)
                            currentActionIndex = availableActions.Count - 1;

                        leftArrowFlash = 0.12f;
                    }
                    else if (scroll < -0.1f)
                    {
                        currentActionIndex++;

                        if (currentActionIndex >= availableActions.Count)
                            currentActionIndex = 0;

                        rightArrowFlash = 0.12f;
                    }

                    leftArrowFlash = Mathf.Max(0f, leftArrowFlash - Time.deltaTime);
                    rightArrowFlash = Mathf.Max(0f, rightArrowFlash - Time.deltaTime);

                    InteractionAction currentAction = availableActions[currentActionIndex];

                    string objectName =
                        interactable.itemData != null
                        ? interactable.itemData.displayName
                        : interactable.gameObject.name;

                    // Додаткова інформація про предмет
                    string itemInfo = "";

                    WorldItem worldItem = interactable.GetComponent<WorldItem>();

                    if (worldItem != null)
                    {
                        ItemInstance instance = worldItem.GetItemInstance();

                        if (instance != null && instance.itemData != null)
                        {
                            string condition = GetConditionLabel(instance);
                            itemInfo = $"{condition} • {instance.itemData.weight:0.0} кг";
                        }
                    }

                    string header = objectName;

                    if (!string.IsNullOrEmpty(itemInfo))
                        header += $"\n{itemInfo}";

                    interactionTextUI.enabled = true;

                    if (availableActions.Count > 1)
                    {
                        string leftArrow =
                            leftArrowFlash > 0
                            ? "<color=#FFD54A>‹</color>"

                            : "<color=#8A8A8A>‹</color>";

                        string rightArrow =
                            rightArrowFlash > 0
                            ? "<color=#FFD54A>›</color>"
                            : "<color=#8A8A8A>›</color>";

                        interactionTextUI.text =
                            $"{header}\n{leftArrow}  [F] {currentAction.displayName}  {rightArrow}";
                    }
                    else
                    {
                        interactionTextUI.text =
                            $"{header}\n[F] {currentAction.displayName}";
                    }

                    if (Keyboard.current.fKey.wasPressedThisFrame)
                    {
                        interactable.Interact(currentAction.type, transform);
                    }
                }
                else
                {
                    interactionTextUI.enabled = false;
                }

                return;
            }
        }

        interactionTextUI.enabled = false;
        currentInteractable = null;
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
}