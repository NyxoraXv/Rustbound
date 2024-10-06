using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject weaponContainerPrefab; // The prefab with InventoryUIInitiator
    [SerializeField] private Transform weaponContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 posAddition; // Position increment for each weapon
    private WeaponManager weaponManager; // Reference to the WeaponManager

    private void OnEnable()
    {
        weaponManager = WeaponManager.Instance; // Get the WeaponManager instance
        PopulateWeaponInventory();
    }

    private void PopulateWeaponInventory()
    {
        // Clear previous entries in the inventory
        foreach (Transform child in weaponContainerParent)
        {
            Destroy(child.gameObject);
        }

        List<int> ownedWeaponIDs = weaponManager.OwnedWeapons; // Get owned weapon IDs
        int i = 0;

        foreach (int weaponID in ownedWeaponIDs)
        {
            // Get weapon data from the WeaponDatabase
            WeaponData weapon = weaponManager.weaponDatabase.GetWeaponByID(weaponID);

            if (weapon != null)
            {
                // Instantiate the weapon prefab and set its parent
                GameObject newWeaponContainer = Instantiate(weaponContainerPrefab, weaponContainerParent);
                newWeaponContainer.GetComponent<RectTransform>().anchoredPosition = (Vector2.zero + (posAddition * i));
                i += 1;

                // Get the InventoryUIInitiator component and initialize it
                InventoryUIInitiator initiator = newWeaponContainer.GetComponent<InventoryUIInitiator>();
                initiator.Initialize(weapon); // Pass the current weapon data to the initializer
            }
        }

        // Optionally, add a message if there are no owned weapons
        if (i == 0)
        {
            Debug.Log("No weapons owned.");
            // Optionally, you can display a UI message indicating no weapons are owned.
        }
    }
}
