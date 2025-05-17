using System.Collections;
using UnityEngine;

public class FPSInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;  // Normal walking speed
    public float sprintSpeed = 10.0f; // Sprinting speed
    public float crouchSpeed = 2.0f; // Crouch speed
    public float jumpForce = 7.0f;  // Jumping strength
    public float gravity = 9.8f;    // Gravity force
    public float crouchHeight = 0.5f; // Crouch height (scaled)
    public float standingHeight = 2.0f; // Standing height

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private bool isSprinting = false;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isAiming = false;
    private bool isFiring = false;
    private float currentHeight;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on the player!");
        }
        currentHeight = standingHeight;
    }

    void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleCrouch();
        HandleAiming();
        HandleFiring();
    }

    void HandleMovement()
    {
        // Prevent sprinting when crouching, aiming, or firing
        if (isCrouching || isAiming || isFiring)
        {
            isSprinting = false; // Disable sprinting in these states
        }
        else
        {
            isSprinting = Input.GetKey(KeyCode.LeftShift); // Sprint when shift is pressed
        }

        // Set the movement speed based on state
        float moveSpeed = isSprinting ? sprintSpeed : (isCrouching ? crouchSpeed : walkSpeed);

        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        moveDirection.x = move.x * moveSpeed;
        moveDirection.z = move.z * moveSpeed;
    }

    void HandleJumping()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            moveDirection.y = jumpForce;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCrouch()
    {
        // Toggle crouch with Left Control key
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouching)
        {
            StartCoroutine(CrouchStand());
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && isCrouching)
        {
            StartCoroutine(StandUp());
        }
    }

    void HandleAiming()
    {
        // Check if the player is aiming (right mouse button)
        isAiming = Input.GetMouseButton(1); // Right-click for aiming
    }

    void HandleFiring()
    {
        // Check if the player is firing (left mouse button)
        isFiring = Input.GetMouseButton(0); // Left-click for firing
    }

    // Smooth crouch down
    IEnumerator CrouchStand()
    {
        float timeElapsed = 0;
        float targetHeight = crouchHeight;
        float startHeight = currentHeight;

        while (timeElapsed < 0.25f)
        {
            currentHeight = Mathf.Lerp(startHeight, targetHeight, timeElapsed / 0.25f);
            characterController.height = currentHeight;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        currentHeight = targetHeight;
        characterController.height = currentHeight;
        isCrouching = true;
    }

    // Smooth stand up
    IEnumerator StandUp()
    {
        float timeElapsed = 0;
        float targetHeight = standingHeight;
        float startHeight = currentHeight;

        while (timeElapsed < 0.25f)
        {
            currentHeight = Mathf.Lerp(startHeight, targetHeight, timeElapsed / 0.25f);
            characterController.height = currentHeight;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        currentHeight = targetHeight;
        characterController.height = currentHeight;
        isCrouching = false;
    }
}
