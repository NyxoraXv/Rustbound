using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIInitiator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, damage, rateOfFire, accuracy;
    [SerializeField] private Image icon;
    [SerializeField] private Button equipButton;
    // [SerializeField] private bool isPrimary;

    private WeaponManager weaponManager;
    private PlayerMovement playerMovement;
    private void Awake() 
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();    
    }

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
            if (weaponManager.isSelectingWeapon)
            {
                weaponManager.OnWeaponSelected(weapon.weaponID); // Select the weapon
                Debug.Log("Selected weapon: " + weapon.weaponName);
                //playerMovement.SetWeapon(weapon.weaponID, WeaponManager.Instance.isPrimary);
            }
            
        }
        else
        {
            Debug.Log("Cannot equip weapon: " + weapon.weaponName + ". Weapon not owned.");
        }
    }
}
