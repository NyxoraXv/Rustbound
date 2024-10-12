using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUIInitiator : MonoBehaviour
{
    [SerializeField] private GameObject itemContainerPrefab; // The prefab with InventoryUIInitiator
    [SerializeField] private Transform itemContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 startPos;

    private TurretManager turretManager; // Reference to the TurretManager

    [SerializeField] private Button showTurretButton; // Button to show turrets

    private Color activeColor = new Color(1f, 1f, 1f); // Color when active (white)
    private Color inactiveColor = new Color(0.588f, 0.588f, 0.588f); // Color when inactive (grey)

    private void Awake()
    {
        turretManager = TurretManager.Instance; // Get the TurretManager instance
        PopulateInventory();

        // Set up button listener for turrets
        showTurretButton.onClick.AddListener(() => PopulateInventory());

        // Update button colors based on the initial state
        UpdateButtonColors();
    }

    private void PopulateInventory()
    {
        // Clear previous entries in the inventory
        foreach (Transform child in itemContainerParent)
        {
            Destroy(child.gameObject);
        }

        // Get the data to display (turrets)
        List<int> itemIDs = turretManager.OwnedTurrets; // Get owned turret IDs
        int i = 0;

        foreach (int itemID in itemIDs)
        {
            // Get the turret data
            TurretData turret = turretManager.turretDatabase.GetTurretByID(itemID);

            if (turret != null)
            {
                // Instantiate the item prefab and set its parent
                GameObject newItemContainer = Instantiate(itemContainerPrefab, itemContainerParent);
                newItemContainer.GetComponent<RectTransform>().anchoredPosition = startPos + new Vector2(0, -i * 50); // Adjust position

                // Get the InventoryUIInitiator component and initialize it
                BuildingDataInitiator initiator = newItemContainer.GetComponent<BuildingDataInitiator>();
                initiator.Initialize(turret); // Pass the current turret data to the initializer

                i++;
            }
        }

        // Optionally, add a message if there are no owned turrets
        if (itemIDs.Count == 0)
        {
            Debug.Log("No turrets owned.");
        }
    }

    // Method to update button colors based on the current state
    private void UpdateButtonColors()
    {
        ColorBlock turretColorBlock = showTurretButton.colors;

        // Set the button color for turrets
        turretColorBlock.normalColor = activeColor;
        turretColorBlock.selectedColor = turretColorBlock.normalColor;
        showTurretButton.colors = turretColorBlock;
    }

    private Color32 targetColor = new Color32();
    private Color32 defaultColor = new Color32();

    public void turretState1()
    {
        // Add your turretState1 logic here
    }

    public void turretState2()
    {
        // Add your turretState2 logic here
    }
}
