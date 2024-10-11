using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [SerializeField] private TextMeshProUGUI hp, sp, current_ammo, max_ammo, level;
    [SerializeField] private Image first_slot, second_slot, healthImage, staminaImage;
    [SerializeField] private PlayerMovement playerMovement; // PlayerMovement inherits from VariableComponent
    [SerializeField] private GameObject profile;
    [SerializeField] private Image icon; // Reference to the icon image

    private void Awake()
    {
        instance = this;
        if (playerMovement == null)
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }

        // Subscribe to the damage event
        if (playerMovement != null)
        {
            playerMovement.OnTakeDamage.AddListener(OnPlayerDamaged);
        }
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateStaminaUI();
    }

    private void UpdateHealthUI()
    {
        // Tween the health bar fill amount smoothly
        healthImage.DOFillAmount(playerMovement._currentHealth / playerMovement.maxHealth, 0.5f);

        // Tween the HP text change smoothly
        DOTween.To(() => int.Parse(hp.text.Split('/')[0]), x => hp.text = $"{x}/{playerMovement.maxHealth}",
                   Mathf.FloorToInt(playerMovement._currentHealth), 0.5f);

        // Trigger low health effects if health is below 20%
        if (playerMovement._currentHealth / playerMovement.maxHealth < 0.2f)
        {
            LoopLowHealthColor();
        }
        else
        {
            // Reset the color to white if health is above 20%
            hp.DOColor(new Color32(255, 255, 255, 255), 0.5f);
        }
    }

    private void UpdateStaminaUI()
    {
        // Tween the health bar fill amount smoothly
        staminaImage.DOFillAmount(playerMovement.currentStamina / playerMovement.maxStamina, 0.5f);

        // Tween the HP text change smoothly
        DOTween.To(() => int.Parse(sp.text.Split('/')[0]), x => sp.text = $"{x}/{playerMovement.maxStamina}",
                   Mathf.FloorToInt(playerMovement.currentStamina), 0.5f);

        // Trigger low health effects if health is below 10%
        if (playerMovement.currentStamina / playerMovement.maxStamina < 0.1f)
        {
            LoopLowStaminaColor();
        }
        else
        {
            // Reset the color to white if health is above 20%
            sp.DOColor(new Color32(255, 255, 255, 255), 0.5f);
        }
    }

    private void LoopLowStaminaColor()
    {
        // Loop the color between white and light grey
        sp.DOColor(new Color32(210, 210, 210, 255), 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    private void LoopLowHealthColor()
    {
        // Loop the color between white and light grey
        hp.DOColor(new Color32(210, 210, 210, 255), 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void ShakeProfile()
    {
        profile.transform.DOShakePosition(0.5f, 10, 10, 90, false, true);
    }

    // This method will be called when the player takes damage
    private void OnPlayerDamaged(float damage)
    {
        ShakeProfile();
        StartCoroutine(AnimateIcon());
    }

    private IEnumerator AnimateIcon()
    {
        // Get the original color of the icon
        Color originalColor = icon.color;
        Color fadedColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f); // Set to light gray

        // Change the icon color to light gray
        icon.DOColor(fadedColor, 0.5f).OnComplete(() =>
        {
            // Change back to the original color
            icon.DOColor(originalColor, 0.5f);
        });

        // Wait for the color change to complete
        yield return new WaitForSeconds(1f); // Wait for a total duration of the animation
    }

    // New method to swap weapon images with animation
    public void SwapWeaponImagesSlot1(Sprite newImage)
    {
        // Animate the scale of the first slot image to give a nice swap effect
        first_slot.transform.DOScale(0.8f, 0.2f).OnComplete(() =>
        {
            first_slot.sprite = newImage; // Swap the image
            first_slot.transform.DOScale(1f, 0.2f);
        });
    }

    public void SwapWeaponImagesSlot2(Sprite newImage)
    {
        // Animate the scale of the second slot image to give a nice swap effect
        second_slot.transform.DOScale(0.8f, 0.2f).OnComplete(() =>
        {
            second_slot.sprite = newImage; // Swap the image
            second_slot.transform.DOScale(1f, 0.2f);
        });
    }


    private void OnDestroy()
    {
        if (playerMovement != null)
        {
            // Unsubscribe from the event when this object is destroyed
            playerMovement.OnTakeDamage.RemoveListener(OnPlayerDamaged);
        }
    }
}
