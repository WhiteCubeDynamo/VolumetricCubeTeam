using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepSoundMaterial : MonoBehaviour
{
    [System.Serializable]
    public struct MaterialFootstepSounds
    {
        public string materialName;
        public AudioClip[] footstepSounds;
    }

    [Header("Footstep Settings")]
    [SerializeField] private MaterialFootstepSounds[] materialSounds;
    [SerializeField] private float stepInterval = 0.5f; // Time between steps when walking
    [SerializeField] private float runStepInterval = 0.3f; // Time between steps when running
    [SerializeField] private float volumeMin = 0.8f;
    [SerializeField] private float volumeMax = 1.0f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;
    
[Header("Fall Settings")]
    [SerializeField] private float fallThreshold = 5f; // Velocity change threshold for landing sound
    [SerializeField] private float maxFallSpeed = 20f; // Maximum fall speed for volume scaling
    [SerializeField] private float fallVolumeMin = 0.5f; // Minimum volume for fall sound
    [SerializeField] private float fallVolumeMax = 1.0f; // Maximum volume for fall sound
    [SerializeField] private AudioClip[] fallSounds;

    [Header("Speed Settings")]
    [SerializeField] private float runSpeedThreshold = 8f; // Speed at which we consider the player running
    
    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundMask = -1;
    
    private AudioSource audioSource;
    private Rigidbody rb;
    private float stepTimer;
    private bool isMoving;
    private bool isGrounded;
    private float currentSpeed;
    private string currentMaterial = "Stone";
    private bool wasGrounded;
    private float previousVelocityY;
    private float velocityChangeOnLanding;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        rb = GetComponent<Rigidbody>();
    }
    
    public void UpdateMovementState(bool moving, bool grounded, float speed)
    {
        isMoving = moving;
        isGrounded = grounded;
        currentSpeed = speed;
        
        // Detect material beneath player
        if (grounded)
        {
            DetectMaterial();
        }
    }
    
    private void DetectMaterial()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance + 1f, groundMask))
        {
            // Check if the hit object has a tag that matches material names
            string tag = hit.collider.tag;
            
            // Check if we have sounds for this tag
            foreach (var materialSound in materialSounds)
            {
                if (materialSound.materialName == tag)
                {
                    currentMaterial = tag;
                    return;
                }
            }
            
            // If no tag match, try to infer from the object name
            string objectName = hit.collider.gameObject.name.ToLower();
            
            if (objectName.Contains("wood"))
            {
                currentMaterial = "Wood";
            }
            else if (objectName.Contains("stone") || objectName.Contains("rock"))
            {
                currentMaterial = "Stone";
            }
            else if (objectName.Contains("metal"))
            {
                currentMaterial = "Metal";
            }
            else if (objectName.Contains("grass"))
            {
                currentMaterial = "Grass";
            }
            // Default to Stone if nothing matches
            else
            {
                currentMaterial = "Stone";
            }
        }
    }
    
void FixedUpdate()
    {
        // Calculate velocity change when landing
        if (!wasGrounded && isGrounded)
        {
            // Calculate the velocity change (impact force)
            velocityChangeOnLanding = Mathf.Abs(previousVelocityY - rb.linearVelocity.y);
            
            // Only play sound if the impact was significant
            if (velocityChangeOnLanding > fallThreshold)
            {
                PlayFallSound();
            }
        }
        
        // Store the previous velocity for next frame
        previousVelocityY = rb.linearVelocity.y;
        wasGrounded = isGrounded;
    }

    void Update()
    {
        if (isMoving && isGrounded)
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
    
    private AudioClip[] GetFootstepSounds()
    {
        foreach (var materialSound in materialSounds)
        {
            if (materialSound.materialName == currentMaterial)
            {
                return materialSound.footstepSounds;
            }
        }
        
        // Return first material sounds as fallback
        if (materialSounds.Length > 0)
        {
            return materialSounds[0].footstepSounds;
        }
        
        return null;
    }

    private void PlayFootstep()
    {
        AudioClip[] footstepSounds = GetFootstepSounds();
        if (footstepSounds == null || footstepSounds.Length == 0) return;

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
        AudioClip[] footstepSounds = GetFootstepSounds();
        if (footstepSounds != null && footstepSounds.Length > 0 && audioSource != null)
        {
            // Play at slightly higher volume for landing
            audioSource.volume = volumeMax;
            audioSource.pitch = Random.Range(0.8f, 0.9f); // Lower pitch for landing
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
        }
}

    private void PlayFallSound()
    {
        if (fallSounds.Length == 0) return;
        
        // Select random fall sound
        AudioClip clip = fallSounds[Random.Range(0, fallSounds.Length)];
        
        // Scale volume based on impact force
        float impactPercent = Mathf.Clamp01((velocityChangeOnLanding - fallThreshold) / (maxFallSpeed - fallThreshold));
        float volume = Mathf.Lerp(fallVolumeMin, fallVolumeMax, impactPercent);
        
        // Play with calculated volume
        audioSource.PlayOneShot(clip, volume);
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 1f));
    }
}
