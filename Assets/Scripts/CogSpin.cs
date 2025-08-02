using UnityEngine;

/// <summary>
/// Spins a gear using a kinematic Rigidbody.
/// Allows full control over the spin axis in local space.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SpinningGear : MonoBehaviour
{
    /// <summary>
    /// Speed of rotation in degrees per second.
    /// </summary>
    [Tooltip("Rotation speed in degrees per second.")]
    public float speed = 60f;

    /// <summary>
    /// Rotation direction:
    /// - true → clockwise
    /// - false → counter-clockwise
    /// </summary>
    [Tooltip("If true, rotates clockwise; if false, counter-clockwise.")]
    public bool side = true;

    /// <summary>
    /// Rotation axis in local space (default = Z-axis: Vector3.forward).
    /// Define X, Y or Z, or a custom direction.
    /// </summary>
    [Tooltip("Rotation axis in local space. Default is Z (forward).")]
    public Vector3 spinAxis = Vector3.forward;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Optionally freeze all position movement
        rb.constraints = RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        // Calculate rotation amount and direction
        float direction = side ? 1f : -1f;
        float rotationThisFrame = speed * Time.fixedDeltaTime * direction;

        // Get axis in world space based on current local orientation
        Vector3 worldAxis = transform.TransformDirection(spinAxis.normalized);

        // Create rotation and apply
        Quaternion deltaRotation = Quaternion.AngleAxis(rotationThisFrame, worldAxis);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
