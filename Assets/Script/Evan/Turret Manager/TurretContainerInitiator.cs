using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretContainerInitiator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, price, damage, rateOfFire, accuracy;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;

    private TurretManager turretManager;

    private void OnEnable()
    {
        turretManager = TurretManager.Instance;
    }

    // Initialize the weapon UI with the given weapon data
    public void Initialize(TurretData turret)
    {
        // Populate the UI fields with the weapon data
        title.text = turret.Name;
        price.text = turret.price.ToString();
        damage.text = turret.Damage.ToString();
        rateOfFire.text = turret.RateOfFire.ToString();
        accuracy.text = turret.Accuracy.ToString("F2"); // Assuming accuracy is a float
        icon.sprite = turret.Image;

        // Add a listener to the buy button to handle weapon purchase
        buyButton.onClick.AddListener(() => TryBuyWeapon(turret));
    }

    // Method to handle weapon purchase
    private void TryBuyWeapon(TurretData weapon)
    {
        bool success = turretManager.buyTurret(weapon.ID);

        if (success)
        {
            Debug.Log("Purchased weapon: " + weapon.Name);
        }
        else
        {
            Debug.Log("Failed to purchase weapon: " + weapon.Name);
        }
    }
}
