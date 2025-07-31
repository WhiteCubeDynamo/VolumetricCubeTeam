using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 25f; // Multiplier for speed
    public float jumpForce = 25f; // Mutliplier for jump
    public Transform groundCheck;
    public float groundDistance = 1f; // Distance to the ground that still allows jumping
    public LayerMask groundMask; // Assign that in inpector to the wanted Layer for detecting ground
    private bool isGrounded;
    public Transform cameraTransform; // Add camera in inspector

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessingJump();
    }

    private void ProcessingJump()
    {
        // Check if player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        Movement();
    }


    private void Movement()
    {
        Vector3 moveDirection = Vector3.zero;
        if (Keyboard.current.wKey.isPressed)
        {
            moveDirection += cameraTransform.forward;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            moveDirection -= cameraTransform.forward;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            moveDirection -= cameraTransform.right;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            moveDirection += cameraTransform.right;
        }

        moveDirection.y = 0f;
        moveDirection.Normalize();

        // Apply movement using Rigidbody physics
        if (moveDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
        }
    }
}
