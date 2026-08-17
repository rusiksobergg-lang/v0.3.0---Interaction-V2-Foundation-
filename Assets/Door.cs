
using UnityEngine;

public class Door : Interactable
{
    [Header("Door Settings")]
    public Transform doorPivot;
    public float openAngle = 90f;
    public float openSpeed = 4f;
    public bool startsLocked = false;

    private bool isOpen = false;
    private bool isLocked;

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private Quaternion targetRotation;

    private void Start()
    {
        isLocked = startsLocked;

        if (doorPivot == null)
            doorPivot = transform;

        closedRotation = doorPivot.localRotation;
        openedRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
        targetRotation = closedRotation;

        UpdateActions();
    }

    private void Update()
    {
        targetRotation = isOpen ? openedRotation : closedRotation;

        doorPivot.localRotation = Quaternion.Slerp(
            doorPivot.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed);
    }

    public override void Interact(InteractionType actionType, Transform interactor)
    {
        switch (actionType)
        {
            case InteractionType.Open:

                if (!isLocked)
                {
                    Vector3 toPlayer = interactor.position - doorPivot.position;

                    float side = Vector3.Dot(doorPivot.right, toPlayer);

                    openedRotation = closedRotation *
                        Quaternion.Euler(0f, side > 0 ? -openAngle : openAngle, 0f);

                    isOpen = true;
                }

                break;

            case InteractionType.Close:
                isOpen = false;
                break;

            case InteractionType.Lock:

                if (!isOpen)
                    isLocked = true;

                break;

            case InteractionType.Unlock:
                isLocked = false;
                break;
        }

        UpdateActions();
    }

    private void UpdateActions()
    {
        actions.Clear();

        if (!isOpen)
        {
            if (isLocked)
            {
                actions.Add(new InteractionAction
                {
                    displayName = "Відкрити ключем",
                    type = InteractionType.Unlock
                });
            }
            else
            {
                actions.Add(new InteractionAction
                {
                    displayName = "Відкрити",
                    type = InteractionType.Open
                });

                actions.Add(new InteractionAction
                {
                    displayName = "Замкнути",
                    type = InteractionType.Lock
                });
            }
        }
        else
        {
            actions.Add(new InteractionAction
            {
                displayName = "Закрити",
                type = InteractionType.Close
            });
        }
    }
}