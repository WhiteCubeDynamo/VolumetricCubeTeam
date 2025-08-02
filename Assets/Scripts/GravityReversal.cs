using UnityEngine;

public class GravityReversal : MonoBehaviour
{
    [SerializeField] private AudioClip gravityReversalSound;
    
    public void GravityRevers()
    {
        Physics.gravity = -Physics.gravity;
        Debug.Log("Gravity reversed");

        // Play sound effect
        if (gravityReversalSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.spatialBlend = 0f; // Ensure 2D audio
            audioSource.PlayOneShot(gravityReversalSound);
        }
    }
}
