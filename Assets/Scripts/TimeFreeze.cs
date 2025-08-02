using UnityEngine;
using System.Collections.Generic;

public class TimeFreeze : MonoBehaviour
{
    [SerializeField] private float freezeDuration = 10f;
    [SerializeField] private AudioClip freezeSound;
    [SerializeField] private AudioClip unfreezeSound;
    
    private bool _isFrozen = false;
    private List<Rigidbody> affectedRigidbodies = new List<Rigidbody>();
    private Dictionary<Rigidbody, bool> originalKinematicStates = new Dictionary<Rigidbody, bool>();
    private Dictionary<Rigidbody, List<MonoBehaviour>> disabledScripts = new Dictionary<Rigidbody, List<MonoBehaviour>>();
    
    public void TimeFreez()
    {
        _isFrozen = !_isFrozen;
        
        if (_isFrozen)
        {
            // Freeze: find all rigidbodies and store their original states
            Rigidbody[] allRigidbodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
            affectedRigidbodies.Clear();
            originalKinematicStates.Clear();
            
            foreach (Rigidbody rb in allRigidbodies)
            {
                if (rb.CompareTag("Player")) continue;
                
                // Store original state and add to affected list
                affectedRigidbodies.Add(rb);
                originalKinematicStates[rb] = rb.isKinematic;
                
                // Apply freeze
                rb.isKinematic = true;
                
                // Disable all scripts on this object
                MonoBehaviour[] scripts = rb.GetComponents<MonoBehaviour>();
                List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>();
                
                foreach (MonoBehaviour script in scripts)
                {
                    if (script != null && script.enabled && script != this)
                    {
                        script.enabled = false;
                        scriptsToDisable.Add(script);
                    }
                }
                
                if (scriptsToDisable.Count > 0)
                {
                    disabledScripts[rb] = scriptsToDisable;
                }
            }
        }
        else
        {
            // Unfreeze: only revert changes to previously affected objects
            foreach (Rigidbody rb in affectedRigidbodies)
            {
                if (rb != null && originalKinematicStates.ContainsKey(rb))
                {
                    rb.isKinematic = originalKinematicStates[rb];
                    
                    // Re-enable previously disabled scripts
                    if (disabledScripts.ContainsKey(rb))
                    {
                        foreach (MonoBehaviour script in disabledScripts[rb])
                        {
                            if (script != null)
                            {
                                script.enabled = true;
                            }
                        }
                    }
                }
            }
            
            // Clear the tracking lists
            affectedRigidbodies.Clear();
            originalKinematicStates.Clear();
            disabledScripts.Clear();
        }
        
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; // Ensure 2D audio
        }
        
        // Play sound based on state
        if (_isFrozen && freezeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(freezeSound);
            Invoke(nameof(TimeFreez), freezeDuration); // Auto-unfreeze
            Debug.Log("Time FREEZED");
        }
        else if (!_isFrozen && unfreezeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(unfreezeSound);
            Debug.Log("Time resumed");
        }
    }
}
