using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    public float distance = 5f;
    public float speed = 2f;
    public float delayAtEnd = 1f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Rigidbody rb;
    bool isWaiting = false;
    bool movingToEnd;
    void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position + distance * Vector3.up;
        rb = GetComponent<Rigidbody>();
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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 fixedStart = Application.isPlaying ? startPosition : transform.position;
        Vector3 fixedEnd = fixedStart + distance * Vector3.up;

        Gizmos.DrawLine(fixedStart, fixedEnd);
        Gizmos.DrawSphere(fixedStart, 0.1f);
        Gizmos.DrawSphere(fixedEnd, 0.1f);
    }
#endif

}
