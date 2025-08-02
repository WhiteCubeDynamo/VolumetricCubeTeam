using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Vector3 dir;
    private CharacterController characterController;
    [SerializeField] float speed = 1;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        dir = new Vector3(Input.GetAxisRaw("Vertical"),0f, Input.GetAxisRaw("Horizontal"));
        characterController.Move(dir * speed * Time.deltaTime);

    }
}
