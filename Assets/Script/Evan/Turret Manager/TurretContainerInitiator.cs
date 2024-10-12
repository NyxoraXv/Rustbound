using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween namespace

public class TurretContainerInitiator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, price, damage, rateOfFire, accuracy;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;

    private TurretManager turretManager;
    private TextMeshProUGUI buyButtonText;

    private void OnEnable()
    {
        turretManager = TurretManager.Instance;

        // Get the TextMeshProUGUI component in the buyButton's children
        buyButtonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Initialize the turret UI with the given turret data
    public void Initialize(TurretData turret)
    {
        // Populate the UI fields with the turret data
        title.text = turret.Name;
        price.text = turret.price.ToString();
        damage.text = turret.Damage.ToString();
        rateOfFire.text = turret.RateOfFire.ToString();
        accuracy.text = turret.Accuracy.ToString("F2"); // Assuming accuracy is a float
        icon.sprite = turret.Image;

        // Check if the turret is owned
        if (turretManager.IsTurretOwned(turret.ID))
        {
            // Set the color to grey for disabled UI elements
            SetOwnedState();
        }
        else
        {
            // Add a listener to the buy button to handle turret purchase
            buyButton.onClick.AddListener(() => TryBuyTurret(turret));
        }
    }

    // Method to handle turret purchase
    private void TryBuyTurret(TurretData turret)
    {
        bool success = turretManager.buyTurret(turret.ID);

        if (success)
        {
            Debug.Log("Purchased turret: " + turret.Name);
            SoundManager.instance.PlaySFX(53);
            SoundManager.instance.PlaySFX(57);
            SetOwnedState();
        }
        else
        {
            SoundManager.instance.PlaySFX(54);
            Debug.Log("Failed to purchase turret: " + turret.Name);
            UIInformation.instance.ShowPopup(UIInformation.PopupMessageType.NotEnoughBalance);
        }
    }

    // Set the UI elements to indicate the turret is owned
    private void SetOwnedState()
    {
        // Change the color to grey (hex: #666666)
        Color greyColor = new Color32(102, 102, 102, 255); // #666666 in RGBA
        buyButton.image.color = greyColor;

        icon.GetComponent<Outline>().effectDistance = Vector2.zero;
        icon.color = new Color32(150, 150, 150, 255);

        // Change the buy button text to "Owned"
        buyButtonText.text = "Owned";

        // Disable the buy button
        buyButton.interactable = false;

        // Animate the icon and outline when owned
        AnimateIcon();
    }

    // Animate the icon's scale and color when owned
    private void AnimateIcon()
    {
        // Get the outline component
        Outline outline = icon.GetComponent<Outline>();

        // Store the original outline effect distance
        Vector2 originalOutlineDistance = outline.effectDistance;

        // Animate the icon scaling and color
        icon.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBounce).OnStart(() =>
        {
            // Tween the icon color to a new color (e.g., bright yellow)
            icon.DOColor(Color.yellow, 0.5f);
            // Tween the outline effect distance to create a glow effect
            outline.effectDistance = new Vector2(2, 2);
            outline.DOEffectDistance(new Vector2(5, 5), 0.5f); // Animate to a larger outline
        }).OnComplete(() =>
        {
            icon.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                // Return to original color and outline after scaling down
                icon.DOColor(new Color32(150, 150, 150, 255), 0.5f);
                outline.effectDistance = originalOutlineDistance; // Reset to original outline distance
            });
        });
    }
}
public static class OutlineExtensions
{
    // Extension method to animate the Outline's effect distance
    public static Tweener DOEffectDistance(this Outline outline, Vector2 endValue, float duration)
    {
        return DOTween.To(
            () => outline.effectDistance,
            x => outline.effectDistance = x,
            endValue,
            duration
        ).SetTarget(outline);
    }
}