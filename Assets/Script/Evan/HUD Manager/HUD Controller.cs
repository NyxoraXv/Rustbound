using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [SerializeField] private TextMeshProUGUI hp, sp, current_ammo, max_ammo, level;
    [SerializeField] private Image first_slot, second_slot, healthImage;
    [SerializeField] private PlayerMovement playerMovement; // PlayerMovement inherits from VariableComponent
    [SerializeField] private GameObject profile;
    [SerializeField] private Image icon; // Reference to the icon image
    [SerializeField] private Material iconMaterial; // Reference to the icon's material

    private static readonly int GlitchFadeID = Shader.PropertyToID("_GlitchFade"); // Cache property ID for performance

    private void Awake()
    {
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
        // Set initial state of the material property and color
        iconMaterial.SetFloat(GlitchFadeID, 0f);
        Color originalColor = icon.color; // Get the original color of the icon
        Color fadedColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f); // Set to light gray

        // Animate the material property from 0 to 1
        iconMaterial.DOFloat(1f, GlitchFadeID, 0.5f).OnUpdate(() =>
        {
            icon.color = Color.Lerp(originalColor, fadedColor, iconMaterial.GetFloat(GlitchFadeID)); // Change color while animating
        });

        // Animate the material property back from 1 to 0
        yield return new WaitForSeconds(0.5f);
        iconMaterial.DOFloat(0f, GlitchFadeID, 0.5f).OnUpdate(() =>
        {
            icon.color = Color.Lerp(fadedColor, originalColor, iconMaterial.GetFloat(GlitchFadeID)); // Change color back
        });

        // Wait until the animation is done
        yield return new WaitForSeconds(0.5f);
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
