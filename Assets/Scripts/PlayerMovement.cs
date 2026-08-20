using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.7f;
    public float runSpeed = 6.8f;
    public float backwardSpeed = 2.2f;
    public float strafeSpeed = 3f;
    public float diagonalSpeed = 5.2f;
    public float crouchMoveSpeed = 1.5f;
    public float proneMoveSpeed = 0.83f;

    [Header("Acceleration")]
    public float acceleration = 6.65f;
    public float deceleration = 14.28f;

    [Header("Jump")]
    public float jumpHeight = 0.94f;
    public float gravity = -9.81f;

    [Header("Look")]
    public float mouseSensitivity = 0.1f;
    public float maxLookAngle = 85f;
    public Transform playerCamera;
    public Transform cameraPivot;

    [Header("Stance")]
    public float standingHeight = 1.7f;
    public float crouchingHeight = 1.2f;
    public float proneHeight = 0.65f;
    public float crouchSpeed = 4f;

    [Header("Camera Height")]
    public float standingCameraHeight = 1.7f;
    public float crouchingCameraHeight = 1.3f;
    public float proneCameraHeight = 0.6f;
    public float cameraSmoothTime = 0.12f;

    private CharacterController controller;
    private PlayerStamina stamina;

    private float cameraPitch;
    private float verticalVelocity;
    private float currentSpeed;
    private Vector3 currentDirection;

    private bool wasSprinting;
    private bool isCrouching;
    private bool isProne;
    private bool isSprinting;

    private float cameraVelocity;

    public bool IsCrouching => isCrouching;
    public bool IsProne => isProne;
    public bool IsSprinting => isSprinting;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stamina = GetComponent<PlayerStamina>();

        if (controller == null)
        {
            Debug.LogError("PlayerMovement: CharacterController is missing.", this);
            enabled = false;
            return;
        }

        if (stamina == null)
        {
            Debug.LogError("PlayerMovement: PlayerStamina is missing.", this);
            enabled = false;
            return;
        }

        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();

            if (cam != null)
                playerCamera = cam.transform;
            else
            {
                Debug.LogError("PlayerMovement: Player Camera is not assigned.", this);
                enabled = false;
                return;
            }
        }

        if (cameraPivot == null)
        {
            Debug.LogError("PlayerMovement: Camera Pivot is not assigned.", this);
            enabled = false;
            return;
        }

        // Player origin is the bottom of the CharacterController.
        // CameraPivot is responsible for eye height.
        // PlayerLean should only rotate CameraPivot.
        controller.center = new Vector3(0f, standingHeight * 0.5f, 0f);
        controller.height = standingHeight;

        cameraPivot.localPosition = new Vector3(
            cameraPivot.localPosition.x,
            standingCameraHeight,
            cameraPivot.localPosition.z
        );
    }

    void Update()
    {
        if (controller == null || !controller.enabled || !gameObject.activeInHierarchy)
            return;

        Look();
        HandleStanceInput();
        UpdateStance();
        Move();
    }

    void Look()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouse = Mouse.current.delta.ReadValue();

        cameraPitch -= mouse.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        // Main Camera handles vertical look.
        // Lean should be handled by PlayerLean, not by this script.
        Vector3 cameraEuler = playerCamera.localEulerAngles;
        cameraEuler.x = cameraPitch;
        playerCamera.localEulerAngles = cameraEuler;

        // Player handles horizontal look.
        transform.Rotate(0f, mouse.x * mouseSensitivity, 0f);
    }

    void HandleStanceInput()
    {
        if (Keyboard.current == null)
            return;

        if (!controller.isGrounded || verticalVelocity > 0f)
            return;

        // C = Standing <-> Crouch
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (isProne)
            {
                // Prone -> Crouch only if the crouching capsule fits.
                if (CanFit(crouchingHeight))
                {
                    isProne = false;
                    isCrouching = true;
                }
            }
            else if (isCrouching)
            {
                // Crouch -> Standing only if the full capsule fits.
                if (CanFit(standingHeight))
                    isCrouching = false;
            }
            else
            {
                isCrouching = true;
            }
        }

        // Z = Standing/Crouch -> Prone, Prone -> Crouch
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            if (isProne)
            {
                if (CanFit(crouchingHeight))
                {
                    isProne = false;
                    isCrouching = true;
                }
            }
            else
            {
                isProne = true;
                isCrouching = false;
            }
        }
    }

    bool CanFit(float targetHeight)
    {
        if (controller == null || !controller.enabled)
            return false;

        float radius = controller.radius;

        // CharacterController не може бути нижчим за свій діаметр
        targetHeight = Mathf.Max(targetHeight, radius * 2f + 0.02f);

        // Невеликий запас над головою
        float margin = 0.05f;

        // Player.position = нижня точка персонажа.
        // Перевіряємо майбутню повну капсулу.
        Vector3 bottom = new Vector3(
     controller.bounds.center.x,
     controller.bounds.min.y + radius,
     controller.bounds.center.z
 );

        Vector3 top = bottom + Vector3.up * (targetHeight - radius * 2f + margin);

        Collider[] hits = Physics.OverlapCapsule(
            bottom,
            top,
            radius,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            // Не враховуємо самого персонажа
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    void UpdateStance()
    {
        float targetHeight;

        if (isProne)
            targetHeight = proneHeight;
        else if (isCrouching)
            targetHeight = crouchingHeight;
        else
            targetHeight = standingHeight;

        float minimumHeight = controller.radius * 2f + 0.02f;
        targetHeight = Mathf.Max(targetHeight, minimumHeight);

        float newHeight = Mathf.MoveTowards(
            controller.height,
            targetHeight,
            crouchSpeed * Time.deltaTime
        );

        controller.height = newHeight;

        // Player origin is at the bottom of the capsule.
        // Therefore the center is always half of the current height.
        controller.center = new Vector3(
            0f,
            newHeight * 0.5f,
            0f
        );

        float targetCameraHeight;

        if (isProne)
            targetCameraHeight = proneCameraHeight;
        else if (isCrouching)
            targetCameraHeight = crouchingCameraHeight;
        else
            targetCameraHeight = standingCameraHeight;

        Vector3 pivotPosition = cameraPivot.localPosition;

        pivotPosition.y = Mathf.SmoothDamp(
            pivotPosition.y,
            targetCameraHeight,
            ref cameraVelocity,
            cameraSmoothTime
        );

        cameraPivot.localPosition = pivotPosition;
    }

    void Move()
    {
        if (controller == null || !controller.enabled || !gameObject.activeInHierarchy)
            return;

        if (Keyboard.current == null)
            return;

        Vector2 input = new Vector2(
            Keyboard.current.dKey.isPressed ? 1f :
            Keyboard.current.aKey.isPressed ? -1f : 0f,

            Keyboard.current.wKey.isPressed ? 1f :
            Keyboard.current.sKey.isPressed ? -1f : 0f
        );

        float targetSpeed = 0f;
        bool sprint = false;

        Vector3 desiredDirection = Vector3.zero;
        bool changingDirection = false;

        if (input.sqrMagnitude > 0.001f)
        {
            desiredDirection =
                transform.right * input.x +
                transform.forward * input.y;

            desiredDirection.Normalize();

            // If the player is moving and the new input points backwards
            // against the current movement direction, brake first.
            if (currentDirection.sqrMagnitude > 0.001f)
            {
                float directionDot =
                    Vector3.Dot(currentDirection, desiredDirection);

                changingDirection = directionDot < -0.5f;
            }

            if (isProne)
            {
                targetSpeed = proneMoveSpeed;
            }
            else if (isCrouching)
            {
                targetSpeed = crouchMoveSpeed;
            }
            else if (
                Keyboard.current.leftShiftKey.isPressed &&
                input.y > 0f &&
                stamina.CanSprint()
            )
            {
                targetSpeed = Mathf.Abs(input.x) > 0.001f
                    ? diagonalSpeed
                    : runSpeed;

                stamina.UseStamina(
                    stamina.drainRate * Time.deltaTime
                );

                sprint = true;
            }
            else if (input.y < 0f)
            {
                targetSpeed = backwardSpeed;
            }
            else if (Mathf.Abs(input.x) > 0.001f)
            {
                targetSpeed = strafeSpeed;
            }
            else
            {
                targetSpeed = walkSpeed;
            }
        }

        if (wasSprinting && !sprint)
            stamina.StopUsingStamina();

        isSprinting = sprint;
        wasSprinting = sprint;

        // When reversing direction, bring speed down to zero first.
        // The new direction is applied only after the player has stopped.
        float speedTarget = changingDirection ? 0f : targetSpeed;

        float speedChange =
            speedTarget > currentSpeed
                ? acceleration
                : deceleration;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            speedTarget,
            speedChange * Time.deltaTime
        );

        if (currentSpeed <= 0.01f)
        {
            currentSpeed = 0f;
            currentDirection = Vector3.zero;
        }

        if (desiredDirection.sqrMagnitude > 0.001f)
        {
            // Normal movement changes direction immediately.
            // A full reversal waits until currentSpeed reaches zero.
            if (!changingDirection || currentSpeed <= 0.01f)
                currentDirection = desiredDirection;
        }

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (
                Keyboard.current.spaceKey.wasPressedThisFrame &&
                !isProne &&
                !isCrouching
            )
            {
                verticalVelocity =
                    Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movement = currentDirection * currentSpeed;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }

}