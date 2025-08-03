using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepSound : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float stepInterval = 0.5f; // Time between steps when walking
    [SerializeField] private float runStepInterval = 0.3f; // Time between steps when running
    [SerializeField] private float volumeMin = 0.8f;
    [SerializeField] private float volumeMax = 1.0f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;
    
    [Header("Speed Settings")]
    [SerializeField] private float runSpeedThreshold = 8f; // Speed at which we consider the player running
    
    private AudioSource audioSource;
    private float stepTimer;
    private bool isMoving;
    private bool isGrounded;
    private float currentSpeed;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    
    public void UpdateMovementState(bool moving, bool grounded, float speed)
    {
        isMoving = moving;
        isGrounded = grounded;
        currentSpeed = speed;
    }
    
    void Update()
    {
        if (isMoving && isGrounded && footstepSounds.Length > 0)
        {
            // Determine step interval based on speed
            float currentStepInterval = currentSpeed > runSpeedThreshold ? runStepInterval : stepInterval;
            
            stepTimer -= Time.deltaTime;
            
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = currentStepInterval;
            }
        }
        else
        {
            // Reset timer when not moving
            stepTimer = 0f;
        }
    }
    
    private void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;
        
        // Random sound selection
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        
        // Random volume and pitch for variation
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        
        audioSource.PlayOneShot(clip);
    }
    
    // Public method to force play a footstep (useful for landing)
    public void PlayLandingSound()
    {
        if (footstepSounds.Length > 0 && audioSource != null)
        {
            // Play at slightly higher volume for landing
            audioSource.volume = volumeMax;
            audioSource.pitch = Random.Range(0.8f, 0.9f); // Lower pitch for landing
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
        }
    }
}
