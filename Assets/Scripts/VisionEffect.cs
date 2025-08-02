using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisionEffect : MonoBehaviour
{
    [SerializeField] private Volume postProcessVolume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        postProcessVolume.profile.TryGet(out vignette);
        postProcessVolume.profile.TryGet(out colorAdjustments);
    }

    public void ApplyAmnesiaVision()
    {
        if (vignette != null)
        {
            vignette.intensity.Override(0.4f);
            vignette.smoothness.Override(0.9f);
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(-0.75f); // darkness
            colorAdjustments.saturation.Override(-50f); // less colors
        }
    }

    public void ResetVision()
    {
        if (vignette != null)
        {
            vignette.intensity.Override(0f);
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(0f);
            colorAdjustments.saturation.Override(0f);
        }
    }
}