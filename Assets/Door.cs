
using UnityEngine;

public class Door : Interactable
{
    [Header("Door Settings")]
    public Transform doorPivot;
    public float openAngle = 90f;
    public float openSpeed = 4f;
    public bool startsLocked = false;
    public Transform player;

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

    public override void Interact(InteractionType actionType)
    {
        switch (actionType)
        {
            case InteractionType.Open:

                if (!isLocked)
                {
                    Vector3 toPlayer = player.position - doorPivot.position;

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

        if (isLocked)
        {
            actions.Add(new InteractionAction
            {
                type = InteractionType.Unlock,
                displayName = "Відкрити ключем"
            });
        }
        else
        {
            if (isOpen)
            {
                actions.Add(new InteractionAction
                {
                    type = InteractionType.Close,
                    displayName = "Закрити"
                });

                actions.Add(new InteractionAction
                {
                    type = InteractionType.Lock,
                    displayName = "Замкнути"
                });
            }
            else
            {
                actions.Add(new InteractionAction
                {
                    type = InteractionType.Open,
                    displayName = "Відкрити"
                });

                actions.Add(new InteractionAction
                {
                    type = InteractionType.Lock,
                    displayName = "Замкнути"
                });
            }
        }
    }
}