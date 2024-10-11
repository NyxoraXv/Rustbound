using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Assuming Universal Render Pipeline (URP) is used
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro

public class Dead : MonoBehaviour
{
    public Volume globalVolume; // Reference to the Global Volume in your scene
    public TextMeshProUGUI deathText; // Reference to the TextMeshPro UI element for the death message
    private DepthOfField depthOfField; // Depth of Field reference

    private void OnEnable()
    {
        if (globalVolume.profile.TryGet(out depthOfField))
        {
            // Start the death effect
            StartDeathEffect();
        }
    }

    void StartDeathEffect()
    {
        // Set initial focal length value for the blur effect
        depthOfField.focalLength.value = 20f;

        // Animate focal length from 20 to 50 over 3 seconds using DoTween
        DOTween.To(() => depthOfField.focalLength.value, x => depthOfField.focalLength.value = x, 50f, 3f)
            .OnComplete(() =>
            {
                // Load the next scene after the blur effect is completed
                SceneManager.LoadScene(1); // Scene build index 1
            });

        // Optional fancy death text effect
        AnimateDeathText();
    }

    void AnimateDeathText()
    {
        // If deathText is assigned, animate it
        if (deathText != null)
        {
            // Set initial scale and alpha
            deathText.rectTransform.localScale = Vector3.zero; // Start from 0 scale
            deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 0); // Make text invisible

            // Animate the text to zoom in and fade in over 1 second
            deathText.rectTransform.DOScale(1.5f, 1f).SetEase(Ease.OutElastic); // Zoom in effect
            deathText.DOFade(1f, 1f); // Fade in the text

            // Optionally, you can add a delay before the text disappears
            DOVirtual.DelayedCall(2f, () =>
            {
                deathText.DOFade(0f, 1f); // Fade out after 2 seconds
            });
        }
    }
}
