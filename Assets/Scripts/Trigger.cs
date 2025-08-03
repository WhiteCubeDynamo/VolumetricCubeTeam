using UnityEngine;

public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PickUp>())
        {
            if(other.GetComponent<PickUp>().key)
            {
                Debug.Log("You Win the game");
            }
            else
            {
                Debug.Log("You Need the Key");
            }
        }
    }
}
