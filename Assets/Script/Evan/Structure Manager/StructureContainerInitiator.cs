using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructureContainerInitiator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, price, damage, rateOfFire, accuracy;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;

    private StructureManager structureManager;

    private void OnEnable()
    {
        structureManager = StructureManager.Instance;
    }

    // Initialize the weapon UI with the given weapon data
    public void Initialize(StructureData structure)
    {
        // Populate the UI fields with the weapon data
        title.text = structure.Name;
        price.text = structure.price.ToString();
        damage.text = structure.hp.ToString();
        icon.sprite = structure.Image;

        // Add a listener to the buy button to handle weapon purchase
        buyButton.onClick.AddListener(() => TryBuyWeapon(structure));
    }

    // Method to handle weapon purchase
    private void TryBuyWeapon(StructureData structure)
    {
        bool success = structureManager.buyStructure(structure.ID);

        if (success)
        {
            Debug.Log("Purchased weapon: " + structure.Name);
        }
        else
        {
            Debug.Log("Failed to purchase weapon: " + structure.Name);
        }
    }
}
