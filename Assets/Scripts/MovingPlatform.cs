using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    public enum MovementAxis { X, Y, Z }
    public MovementAxis movementAxis = MovementAxis.Y;
    public bool invertDirection = false;

    public float distance = 5f;
    public float speed = 2f;
    public float delayAtEnd = 1f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Rigidbody rb;
    private bool isWaiting = false;
    private bool movingToEnd;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        endPosition = startPosition + GetDirectionVector() * distance;
    }

    void FixedUpdate()
    {
        if (isWaiting) return;

        Vector3 target = movingToEnd ? endPosition : startPosition;
        Vector3 newPos = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector3.Distance(rb.position, target) < 0.01f)
        {
            StartCoroutine(WaitAndReverseDirection());
        }
    }

    private IEnumerator WaitAndReverseDirection()
    {
        isWaiting = true;
        yield return new WaitForSeconds(delayAtEnd);
        movingToEnd = !movingToEnd;
        isWaiting = false;
    }

    private Vector3 GetDirectionVector()
    {
        Vector3 dir = Vector3.zero;
        switch (movementAxis)
        {
            case MovementAxis.X: dir = Vector3.right; break;
            case MovementAxis.Y: dir = Vector3.up; break;
            case MovementAxis.Z: dir = Vector3.forward; break;
        }

        return invertDirection ? -dir : dir;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 fixedStart = Application.isPlaying ? startPosition : transform.position;
        Vector3 fixedEnd = fixedStart + GetDirectionVector() * distance;

        Gizmos.DrawLine(fixedStart, fixedEnd);
        Gizmos.DrawSphere(fixedStart, 0.1f);
        Gizmos.DrawSphere(fixedEnd, 0.1f);
    }
#endif
}
