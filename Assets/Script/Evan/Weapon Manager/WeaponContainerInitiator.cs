using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponContainerInitiator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, price, damage, rateOfFire, accuracy;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;

    private WeaponManager weaponManager;
    private TextMeshProUGUI buyButtonText;

    private void OnEnable()
    {
        weaponManager = WeaponManager.Instance;

        // Get the TextMeshProUGUI component in the buyButton's children
        buyButtonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Initialize the weapon UI with the given weapon data
    public void Initialize(WeaponData weapon)
    {
        // Populate the UI fields with the weapon data
        title.text = weapon.weaponName;
        price.text = weapon.price.ToString();
        damage.text = weapon.weaponDamage.ToString();
        rateOfFire.text = weapon.weaponRateOfFire.ToString();
        accuracy.text = weapon.weaponAccuracy.ToString("F2"); // Assuming accuracy is a float
        icon.sprite = weapon.weaponImage;

        // Check if the weapon is owned
        if (weaponManager.IsWeaponOwned(weapon.weaponID))
        {
            // Set the color to 666666 (grey) for disabled UI elements
            SetOwnedState();
        }
        else
        {
            // Add a listener to the buy button to handle weapon purchase
            buyButton.onClick.AddListener(() => TryBuyWeapon(weapon));
        }
    }

    // Method to handle weapon purchase
    private void TryBuyWeapon(WeaponData weapon)
    {
        bool success = weaponManager.BuyWeapon(weapon.weaponID);

        if (success)
        {
            Debug.Log("Purchased weapon: " + weapon.weaponName);
            SetOwnedState();
            SoundManager.instance.PlaySFX(17);
        }
        else
        {
            SoundManager.instance.PlaySFX(18);
            Debug.Log("Failed to purchase weapon: " + weapon.weaponName);
        }
    }

    // Set the UI elements to indicate the weapon is owned
    private void SetOwnedState()
    {
        // Change the color to grey (hex: #666666)
        Color greyColor = new Color32(102, 102, 102, 255); // #666666 in RGBA
        buyButton.image.color = greyColor;

        // Change the buy button text to "Owned"
        buyButtonText.text = "Owned";

        // Disable the buy button
        buyButton.interactable = false;
    }
}
