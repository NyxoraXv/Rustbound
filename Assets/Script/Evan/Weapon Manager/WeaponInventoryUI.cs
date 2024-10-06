using System.Collections.Generic;
using UnityEngine;

public class WeaponInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject weaponContainerPrefab; // The prefab with WeaponContainerInitiator
    [SerializeField] private Transform weaponContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 posAddition;
    private WeaponDatabase weaponDatabase;    // The database of weapons
    

    private void OnEnable()
    {
        weaponDatabase = WeaponManager.Instance.weaponDatabase;
        PopulateWeaponInventory();
    }

    private void PopulateWeaponInventory()
    {
        List<WeaponData> allWeapons = weaponDatabase.GetAllWeapons();
        int i = 0;
        foreach (WeaponData weapon in allWeapons)
        {
            // Instantiate the weapon prefab and set its parent
            GameObject newWeaponContainer = Instantiate(weaponContainerPrefab, weaponContainerParent);
            newWeaponContainer.GetComponent<RectTransform>().anchoredPosition = (Vector2.zero+(posAddition*i));
            i += 1;
            // Get the WeaponContainerInitiator component and initialize it
            WeaponContainerInitiator initiator = newWeaponContainer.GetComponent<WeaponContainerInitiator>();
            initiator.Initialize(weapon); // Pass the current weapon data to the initializer
        }
    }
}
