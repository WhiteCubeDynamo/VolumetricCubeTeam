using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VerticalMovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    public bool startMovingUp = true;

    private Rigidbody rb;
    private Vector3 startPos;
    private bool movingUp;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;

        startPos = transform.position;
        movingUp = startMovingUp; // set initial direction
    }

    void FixedUpdate()
    {
        float topY = startPos.y + moveDistance;
        float bottomY = startPos.y;

        // Switch direction at bounds
        if (movingUp && transform.position.y >= topY)
            movingUp = false;
        else if (!movingUp && transform.position.y <= bottomY)
            movingUp = true;

        // Apply velocity
        Vector3 velocity = Vector3.up * moveSpeed * (movingUp ? 1 : -1);
        rb.linearVelocity = velocity;
    }

    void OnDrawGizmos()
    {
        Vector3 gizmoStart = Application.isPlaying ? startPos : transform.position;
        Vector3 gizmoEnd = gizmoStart + Vector3.up * moveDistance;

        // Start point: Green
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(gizmoStart, 0.2f);

        // End point: Red
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gizmoEnd, 0.2f);

        // Path line: Yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gizmoStart, gizmoEnd);
    }
}
