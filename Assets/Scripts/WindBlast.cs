using UnityEngine;
using System.Collections;

public class WindBlast : MonoBehaviour
{
    [SerializeField] private AudioClip windSound;
    [SerializeField] private int windLoops = 10;
    [SerializeField] private float minForceStrength = 5f;
    [SerializeField] private float maxForceStrength = 20f;
    [SerializeField] private float minWaitTime = 0.5f;
    [SerializeField] private float maxWaitTime = 2f;
    [SerializeField] private float verticalVariation = 0.2f;
    
    public void ApplyWind()
    {
        if (windSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.spatialBlend = 0f; // Ensure 2D audio
            audioSource.PlayOneShot(windSound);
        }
        Debug.Log("Apply Wind");
        StartCoroutine(RandomWindRoutine());
    }

    private IEnumerator RandomWindRoutine()
    {
        int loops = windLoops;
        while (loops>0)
        {
            loops--;
            // Generate random direction and strength
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-verticalVariation, verticalVariation),  // Configurable vertical variation
                Random.Range(-1f, 1f)
            ).normalized;

            float randomStrength = Random.Range(minForceStrength, maxForceStrength);

            // Apply to all rigidbodies
            foreach (Rigidbody rb in FindObjectsByType<Rigidbody>(FindObjectsSortMode.None))
            {
                if (!rb.isKinematic)  // Don't affect frozen objects
                {
                    rb.AddForce(randomDirection * randomStrength * Time.deltaTime, ForceMode.Force);
                }
            }

            // Wait before changing wind again
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }
}
