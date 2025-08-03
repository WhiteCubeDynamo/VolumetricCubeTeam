using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [Header("Look Settings")]
    [Tooltip("Base mouse sensitivity multiplier")]
    public float baseSensitivity = 20f;

    [Tooltip("Minimum sensitivity value")]
    public float minSensitivity = 5f;

    [Tooltip("Maximum sensitivity value")]
    public float maxSensitivity = 50f;

    [Tooltip("Scroll wheel sensitivity adjustment speed")]
    public float scrollSensitivity = 2f;

    [Tooltip("Minimum vertical angle (looking down) in degrees")]
    public float minPitch = -80f;

    [Tooltip("Maximum vertical angle (looking up) in degrees")]
    public float maxPitch = 80f;

    [Tooltip("Invert Y axis")]
    public bool invertY = false;

    // Current sensitivity (modified by scroll wheel)
    private float currentSensitivity;

    // internal state
    private float yaw;   // horizontal rotation
    private float pitch; // vertical rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSensitivity = baseSensitivity;
    }

    void Update()
    {
        AdjustSensitivity();
        LookAround();
    }

    private void AdjustSensitivity()
    {
        float scrollValue = Mouse.current.scroll.y.ReadValue();
        currentSensitivity += scrollValue * scrollSensitivity * Time.deltaTime;
        currentSensitivity = Mathf.Clamp(currentSensitivity, minSensitivity, maxSensitivity);
    }

    private void LookAround()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float deltaX = mouseDelta.x * currentSensitivity * Time.deltaTime;
        float deltaY = mouseDelta.y * currentSensitivity * Time.deltaTime * (invertY ? 1f : -1f);

        yaw += deltaX;
        pitch += deltaY;

        // Clamp pitch to avoid flipping
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Optionally wrap yaw to avoid large numbers (not strictly necessary)
        yaw = Mathf.Repeat(yaw, 360f);

        // Apply rotation: pitch affects X (look up/down), yaw affects Y (turn left/right)
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
