using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretContainerInitiator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, price, damage, rateOfFire, accuracy;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;

    private WeaponManager weaponManager;

    private void OnEnable()
    {
        weaponManager = WeaponManager.Instance;
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

        // Add a listener to the buy button to handle weapon purchase
        buyButton.onClick.AddListener(() => TryBuyWeapon(weapon));
    }

    // Method to handle weapon purchase
    private void TryBuyWeapon(WeaponData weapon)
    {
        bool success = weaponManager.BuyWeapon(weapon.weaponID);

        if (success)
        {
            Debug.Log("Purchased weapon: " + weapon.weaponName);
        }
        else
        {
            Debug.Log("Failed to purchase weapon: " + weapon.weaponName);
        }
    }
}
