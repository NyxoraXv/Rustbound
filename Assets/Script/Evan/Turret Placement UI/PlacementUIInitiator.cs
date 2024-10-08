using System.Collections.Generic;
using UnityEngine;

public class PlacementUIInitiator : MonoBehaviour
{
    // Common inventory UI settings
    [SerializeField] private GameObject itemContainerPrefab; // The prefab with InventoryUIInitiator
    [SerializeField] private Transform itemContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 startPos;

    private StructureManager structureManager; // Reference to the StructureManager
    private TurretManager turretManager; // Reference to the TurretManager

    [SerializeField] private bool showStructures = true; // Flag to toggle between structures and turrets

    private void OnEnable()
    {
        structureManager = StructureManager.Instance; // Get the StructureManager instance
        turretManager = TurretManager.Instance; // Get the TurretManager instance
        PopulateInventory();
    }

    private void PopulateInventory()
    {
        // Clear previous entries in the inventory
        foreach (Transform child in itemContainerParent)
        {
            Destroy(child.gameObject);
        }

        // Get the data to display (structures or turrets)
        List<int> itemIDs = GetItemIDs(); // Get the IDs of the items to display
        int i = 0;

        foreach (int itemID in itemIDs)
        {
            // Get the item data
            object itemData = GetItemData(itemID);

            if (itemData != null)
            {
                // Instantiate the item prefab and set its parent
                GameObject newItemContainer = Instantiate(itemContainerPrefab, itemContainerParent);
                newItemContainer.GetComponent<RectTransform>().anchoredPosition = startPos + new Vector2(0, -i * 50); // Adjust position

                // Get the InventoryUIInitiator component and initialize it
                BuildingDataInitiator initiator = newItemContainer.GetComponent<BuildingDataInitiator>();

                if (showStructures)
                {
                    StructureData structure = (StructureData)itemData;
                    initiator.Initialize(structure); // Pass the current structure data to the initializer
                }
                else
                {
                    TurretData turret = (TurretData)itemData;
                    initiator.Initialize(turret); // Pass the current turret data to the initializer
                }

                i++;
            }
        }

        // Optionally, add a message if there are no owned items
        if (itemIDs.Count == 0)
        {
            Debug.Log("No items owned.");
            // Optionally, you can display a UI message indicating no items are owned.
        }
    }

    // Method to get the IDs of the items to display
    private List<int> GetItemIDs()
    {
        if (showStructures)
        {
            return structureManager.OwnedStructure; // Get owned structure IDs
        }
        else
        {
            return turretManager.OwnedTurrets; // Get owned turret IDs
        }
    }

    // Method to get the item data
    private object GetItemData(int itemID)
    {
        if (showStructures)
        {
            return structureManager.structureDatabase.GetStructureByID(itemID);
        }
        else
        {
            return turretManager.turretDatabase.GetTurretByID(itemID);
        }
    }

    // New method to update the UI based on the index
    public void UpdateUI(int index)
    {
        // Determine whether to show turrets or structures based on the index
        if (index == 0)
        {
            showStructures = false; // Show turrets
        }
        else if (index == 1)
        {
            showStructures = true; // Show structures
        }
        else
        {
            Debug.LogWarning("Invalid index. Use 0 for turrets and 1 for structures.");
            return;
        }

        // Refresh the UI
        PopulateInventory();
    }
}