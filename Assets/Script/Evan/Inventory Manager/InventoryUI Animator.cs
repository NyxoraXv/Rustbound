using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Make sure to include this for DOTween

public class InventoryUIAnimator : MonoBehaviour
{
    [SerializeField] private Image primarySlot, secondarySlot, primaryIcon, secondaryIcon;
    private Color32 enabledColor = new Color32(255, 255, 255, 255);
    private Color32 disabledColor = new Color32(140, 140, 140, 255);

    private static readonly int HologramFadeID = Shader.PropertyToID("_HologramFade");

    public void TriggerPrimarySlot(bool state)
    {
        if (state)
        {
            primarySlot.DOColor(enabledColor, 0.2f); // Animate to enabled color over 0.2 seconds
            primaryIcon.transform.DOScale(1.1f, 0.2f); // Scale up to 1.1 over 0.2 seconds

            // Set the float value for the primary icon material
            SetHologramFade(primaryIcon, 1f); // Set to 1 when enabled
        }
        else
        {
            primarySlot.DOColor(disabledColor, 0.2f); // Animate to disabled color over 0.2 seconds
            primaryIcon.transform.DOScale(1.0f, 0.2f); // Scale back to 1.0 over 0.2 seconds

            // Set the float value for the primary icon material
            SetHologramFade(primaryIcon, 0f); // Set to 0 when disabled
        }
    }

    public void TriggerSecondarySlot(bool state)
    {
        if (state)
        {
            secondarySlot.DOColor(enabledColor, 0.2f); // Animate to enabled color over 0.2 seconds
            secondaryIcon.transform.DOScale(1.1f, 0.2f); // Scale up to 1.1 over 0.2 seconds

            // Set the float value for the secondary icon material
            SetHologramFade(secondaryIcon, 1f); // Set to 1 when enabled
        }
        else
        {
            secondarySlot.DOColor(disabledColor, 0.2f); // Animate to disabled color over 0.2 seconds
            secondaryIcon.transform.DOScale(1.0f, 0.2f); // Scale back to 1.0 over 0.2 seconds

            // Set the float value for the secondary icon material
            SetHologramFade(secondaryIcon, 0f); // Set to 0 when disabled
        }
    }

    private void SetHologramFade(Image icon, float value)
    {
        Material iconMaterial = icon.material; // Get the material from the Image component
        if (iconMaterial != null)
        {
            iconMaterial.SetFloat(HologramFadeID, value); // Set the _HologramFade property
        }
    }
}
