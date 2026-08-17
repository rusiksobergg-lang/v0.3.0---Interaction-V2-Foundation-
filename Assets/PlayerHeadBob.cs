using UnityEngine;

public class PlayerHeadBob : MonoBehaviour
{
    public float bobSpeed = 8f;
    public float bobAmount = 0.03f;
    public float sideBobAmount = 0.015f;

    private Vector3 startPos;
    private CharacterController controller;
    private float timer;
    private PlayerMovement movement;


    void Start()
    {
        startPos = transform.localPosition;
        controller = GetComponentInParent<CharacterController>();
        movement = GetComponentInParent<PlayerMovement>();
    }
    void Update()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            float speedMultiplier = 1f;
            float amountMultiplier = 1f;

            if (movement.IsSprinting)
            {
                speedMultiplier = 1.5f;
                amountMultiplier = 1.5f;
            }
            else if (movement.IsCrouching)
            {
                speedMultiplier = 0.7f;
                amountMultiplier = 0.5f;
            }
            else if (movement.IsProne)
            {
                speedMultiplier = 0.3f;
                amountMultiplier = 0.2f;
            }

            timer += Time.deltaTime * bobSpeed * speedMultiplier;

            Vector3 pos = startPos;

            // Вгору-вниз
            pos.y += Mathf.Sin(timer) * bobAmount * amountMultiplier;
            // Вліво-вправо
            pos.x += Mathf.Sin(timer * 2f) * sideBobAmount * amountMultiplier;
            transform.localPosition = pos;
        }
        else
        {
            timer = Mathf.Lerp(timer, 0f, Time.deltaTime * 6f);
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos,
                Time.deltaTime * bobSpeed
            );
        }
    }
}
