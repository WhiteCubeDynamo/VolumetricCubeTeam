using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    Vector2 turn; // Stores accumulated mouse movement
    float sensitivity = 20f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        LookAround();
    }

    private void LookAround()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue(); // Read mouse value

        turn.x += mouseDelta.x * sensitivity * Time.deltaTime;
        turn.y += mouseDelta.y * sensitivity * Time.deltaTime;

        // Apply rotation to camera
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0f);
    }
}
