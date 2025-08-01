using UnityEngine;

public class GravityReversal : MonoBehaviour
{
    public void GravityRevers()
    {
        Physics.gravity = -1 * Physics.gravity;
        Debug.Log("reverse gravity");
    }
}
