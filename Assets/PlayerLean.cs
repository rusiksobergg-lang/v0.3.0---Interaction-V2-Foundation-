using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLean : MonoBehaviour
{
    public float leanAngle = 12f;
    public float leanOffset = 0.18f;
    public float leanSpeed = 8f;

    private float currentLean;
    private float currentOffset;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float targetAngle = 0f;
        float targetOffset = 0f;

        if (Keyboard.current.qKey.isPressed)
        {
            targetAngle = leanAngle;
            targetOffset = -leanOffset;
        }
        else if (Keyboard.current.eKey.isPressed)
        {
            targetAngle = -leanAngle;
            targetOffset = leanOffset;
        }

        currentLean = Mathf.Lerp(currentLean, targetAngle, leanSpeed * Time.deltaTime);
        currentOffset = Mathf.Lerp(currentOffset, targetOffset, leanSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(0f, 0f, currentLean);

        Vector3 pos = transform.localPosition;
        pos.x = startPos.x + currentOffset;
        transform.localPosition = pos;
    }
}