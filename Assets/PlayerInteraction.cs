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

                    InteractionAction currentAction =
    availableActions[currentActionIndex];

                    interactionTextUI.enabled = true;
                    if (availableActions.Count > 1)
                    {
                        string leftArrow = leftArrowFlash > 0
    ? "<color=#FFD54A><</color>"
    : "<color=#8A8A8A><</color>";

                        string rightArrow = rightArrowFlash > 0
                            ? "<color=#FFD54A>></color>"
                            : "<color=#8A8A8A>></color>";

                        interactionTextUI.text =
                            $"{leftArrow}  [F] {currentAction.displayName}  {rightArrow}";
                    }
                    else
                    {
                        interactionTextUI.text =
                            $"[F] {currentAction.displayName}";
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
    }
}