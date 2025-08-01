using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 25f; // Multiplier for speed 
    public float jumpForce = 25f; // Multiplier for jump 
    public float groundDistance = 1f; // Distance to the ground that still allows jumping 
    public LayerMask groundMask; // Assign that in inspector to the wanted Layer for detecting ground 
    private bool isGrounded;
    public Transform cameraTransform; // Add camera in inspector 

    [Header("Movement Settings")]
    public float maxSpeed = 10f; // Maximum horizontal speed 
    public float friction = 10f; // How quickly the player stops when no input 
    public AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 1, 1, 0); // High at 0 speed, low at max speed 

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
        // Raycast version of ground check 
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance + 0.1f, groundMask);

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

        // Get horizontal velocity only (ignore Y component) 
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        if (moveDirection != Vector3.zero)
        {
            // Calculate speed in the movement direction 
            float speedInMoveDirection = Vector3.Dot(horizontalVelocity, moveDirection);
            speedInMoveDirection = Mathf.Max(0, speedInMoveDirection); // Only consider forward speed 

            // Calculate force multiplier based on speed in movement direction 
            float forceMultiplier = accelerationCurve.Evaluate(Mathf.Clamp01(speedInMoveDirection / maxSpeed));
            rb.AddForce(moveDirection * speed * forceMultiplier, ForceMode.Force);

            // Resist perpendicular movement - apply friction to velocity not aligned with moveDirection 
            Vector3 perpVelocity = horizontalVelocity - Vector3.Project(horizontalVelocity, moveDirection);
            if (perpVelocity.magnitude > 0.1f)
            {
                rb.AddForce(-perpVelocity * friction, ForceMode.Force);
            }
        }
        else
        {
            // Apply friction to all movement when no input 
            if (currentSpeed > 0.1f)
            {
                rb.AddForce(-horizontalVelocity * friction, ForceMode.Force);
            }
            else
            {
                // Completely stop when speed is very low 
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MovingPlatform>())
        {
            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        transform.parent = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundDistance + 0.1f));
    }
}
