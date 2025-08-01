using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MovementAxis { X, Y, Z }
    public MovementAxis movementAxis = MovementAxis.X;

    public float distance = 5f;
    public bool invertDirection = false;
    public float speed = 2f;

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance);

        if (invertDirection)
            offset *= -1f;

        Vector3 newPosition = _startPosition;
        switch (movementAxis)
        {
            case MovementAxis.X:
                newPosition.x += offset;
                break;
            case MovementAxis.Y:
                newPosition.y += offset;
                break;
            case MovementAxis.Z:
                newPosition.z += offset;
                break;
        }

        transform.position = newPosition;
    }
}
