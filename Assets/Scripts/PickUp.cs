using UnityEngine;

public class PickUp : MonoBehaviour
{
    public bool key = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Key>())
        {
            Debug.Log("Key Colllected");
            key = true;
        }
        else if(other.GetComponent<Key>())
        {

        }
    }
}
