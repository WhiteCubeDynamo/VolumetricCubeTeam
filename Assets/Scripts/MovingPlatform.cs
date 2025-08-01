using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    public enum MovementAxis { X, Y, Z }
    public MovementAxis movementAxis = MovementAxis.X;

    public float distance = 5f;
    public float speed = 2f;
    public float delayAtEnd = 1f;
    public bool invertDirection = false;

    private Vector3 startPosition;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float currentOffset = 0f;
    private int directionSign = 1;
    private bool isWaiting = false;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        // Get local movement direction based on selected axis
        switch (movementAxis)
        {
            case MovementAxis.X: moveDirection = transform.right; break;
            case MovementAxis.Y: moveDirection = transform.up; break;
            case MovementAxis.Z: moveDirection = transform.forward; break;
        }

        if (invertDirection) directionSign = -1;
    }

    void FixedUpdate()
    {
        if (isWaiting) return;

        // Move the offset based on direction and speed
        currentOffset += speed * directionSign * Time.fixedDeltaTime;

        // Clamp and reverse when limits are reached
        if (Mathf.Abs(currentOffset) >= distance / 2f)
        {
            currentOffset = Mathf.Clamp(currentOffset, -distance / 2f, distance / 2f);
            StartCoroutine(WaitAndReverseDirection());
        }

        Vector3 targetPosition = startPosition + moveDirection * currentOffset;
        rb.MovePosition(targetPosition);
    }

    private IEnumerator WaitAndReverseDirection()
    {
        isWaiting = true;
        yield return new WaitForSeconds(delayAtEnd);
        directionSign *= -1;
        isWaiting = false;
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Choose direction based on selected axis
        Vector3 dir = Vector3.right;
        switch (movementAxis)
        {
            case MovementAxis.X: dir = transform.right; break;
            case MovementAxis.Y: dir = transform.up; break;
            case MovementAxis.Z: dir = transform.forward; break;
        }

        if (invertDirection)
            dir *= -1;

        // Calculate endpoints of movement
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Vector3 from = center - dir.normalized * (distance / 2f);
        Vector3 to = center + dir.normalized * (distance / 2f);

        // Draw path line
        Gizmos.color = Color.green;
        Gizmos.DrawLine(from, to);

        // Draw arrow at the center indicating direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, dir.normalized);
    }
#endif

}

