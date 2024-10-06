using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIInitiator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, damage, rateOfFire, accuracy;
    [SerializeField] private Image icon;
    [SerializeField] private Button equipButton;

    private WeaponManager weaponManager;

    private void OnEnable()
    {
        weaponManager = WeaponManager.Instance;
    }

    // Initialize the inventory UI with the given weapon data
    public void Initialize(WeaponData weapon)
    {
        // Populate the UI fields with the weapon data
        title.text = weapon.weaponName;
        damage.text = weapon.weaponDamage.ToString();
        rateOfFire.text = weapon.weaponRateOfFire.ToString();
        accuracy.text = weapon.weaponAccuracy.ToString("F2"); // Assuming accuracy is a float
        icon.sprite = weapon.weaponImage;

        // Clear any existing listeners before adding a new one
        equipButton.onClick.RemoveAllListeners();

        // Add a listener to the equip button to handle weapon equipping
        equipButton.onClick.AddListener(() => TryEquipWeapon(weapon));
    }

    // Method to handle weapon equipping
    private void TryEquipWeapon(WeaponData weapon)
    {
        // Equip the weapon if owned
        if (weaponManager.IsWeaponOwned(weapon.weaponID))
        {
            weaponManager.EquipWeapon(weapon.weaponID, 1); // You can choose the slot as needed
            Debug.Log("Equipped weapon: " + weapon.weaponName);
        }
        else
        {
            Debug.Log("Cannot equip weapon: " + weapon.weaponName + ". Weapon not owned.");
        }
    }
}
