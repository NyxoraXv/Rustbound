using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public WeaponDatabase weaponDatabase; // Assign this in the Inspector.

    // Equipped weapon slots (2 slots)
    private WeaponData equippedWeaponSlot1;
    private WeaponData equippedWeaponSlot2;

    public Image weaponSlot1Image; // UI Image for equipped weapon slot 1
    public Image weaponSlot2Image; // UI Image for equipped weapon slot 2

    private List<int> ownedWeapons = new List<int>(); // Store owned weapon IDs

    private bool isSelectingWeapon = false; // Indicates whether the player is in weapon selection mode

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Method to buy a weapon by ID
    public bool BuyWeapon(int weaponID)
    {
        WeaponData weaponToBuy = weaponDatabase.GetWeaponByID(weaponID);

        if (weaponToBuy != null && !IsWeaponOwned(weaponID))
        {
            if (CurrencyManager.Instance.SpendCurrency(weaponToBuy.price))
            {
                ownedWeapons.Add(weaponID); // Add weapon ID to the owned list
                Debug.Log("Weapon bought: " + weaponToBuy.weaponName);
                return true;
            }
            else
            {
                Debug.Log("Not enough currency to buy " + weaponToBuy.weaponName);
                return false;
            }
        }
        else
        {
            Debug.Log("Weapon already owned or not found.");
            return false;
        }
    }

    // Equip a weapon into one of the two slots by ID
    public void EquipWeapon(int weaponID, int slot)
    {
        if (IsWeaponOwned(weaponID))
        {
            WeaponData weaponToEquip = weaponDatabase.GetWeaponByID(weaponID);

            // Check which slot to equip
            if (slot == 1)
            {
                // Replace if already equipped
                if (equippedWeaponSlot1 != null)
                {
                    Debug.Log("Replacing weapon in Slot 1: " + equippedWeaponSlot1.weaponName);
                }

                equippedWeaponSlot1 = weaponToEquip;
                weaponSlot1Image.sprite = weaponToEquip.weaponImage; // Update UI for slot 1
                Debug.Log("Weapon equipped in Slot 1: " + equippedWeaponSlot1.weaponName);
            }
            else if (slot == 2)
            {
                // Replace if already equipped
                if (equippedWeaponSlot2 != null)
                {
                    Debug.Log("Replacing weapon in Slot 2: " + equippedWeaponSlot2.weaponName);
                }

                equippedWeaponSlot2 = weaponToEquip;
                weaponSlot2Image.sprite = weaponToEquip.weaponImage; // Update UI for slot 2
                Debug.Log("Weapon equipped in Slot 2: " + equippedWeaponSlot2.weaponName);
            }
        }
        else
        {
            Debug.Log("Weapon not owned.");
        }
    }

    // Unequip weapon from a slot
    public void UnequipWeapon(int slot)
    {
        if (slot == 1)
        {
            if (equippedWeaponSlot1 != null)
            {
                Debug.Log("Unequipped weapon from Slot 1: " + equippedWeaponSlot1.weaponName);
            }
            equippedWeaponSlot1 = null;
            weaponSlot1Image.sprite = null; // Clear the image for slot 1
        }
        else if (slot == 2)
        {
            if (equippedWeaponSlot2 != null)
            {
                Debug.Log("Unequipped weapon from Slot 2: " + equippedWeaponSlot2.weaponName);
            }
            equippedWeaponSlot2 = null;
            weaponSlot2Image.sprite = null; // Clear the image for slot 2
        }
    }

    // Check if a weapon is owned by ID
    public bool IsWeaponOwned(int weaponID)
    {
        return ownedWeapons.Contains(weaponID);
    }

    // Method to get the equipped weapon in a slot (1 or 2)
    public WeaponData GetEquippedWeapon(int slot)
    {
        if (slot == 1)
        {
            return equippedWeaponSlot1;
        }
        else if (slot == 2)
        {
            return equippedWeaponSlot2;
        }
        return null;
    }

    // Start weapon selection mode
    public void BeginSelectWeapon()
    {
        isSelectingWeapon = true;
        Debug.Log("Entering weapon selection mode.");
        // Additional UI setup can be done here, such as opening a weapon selection screen
    }

    // Handle weapon selection in UI
    public void OnWeaponSelected(int weaponID)
    {
        // Attempt to equip the selected weapon in the first available slot
        if (equippedWeaponSlot1 == null)
        {
            EquipWeapon(weaponID, 1);
        }
        else if (equippedWeaponSlot2 == null)
        {
            EquipWeapon(weaponID, 2);
        }
        else
        {
            // If both slots are occupied, replace the one in slot 1 (or you can customize this logic)
            EquipWeapon(weaponID, 1);
        }
    }

    // Exit weapon selection mode
    public void ExitSelectWeapon()
    {
        isSelectingWeapon = false;
        Debug.Log("Exiting weapon selection mode.");
        // Additional UI teardown can be done here, such as closing the weapon selection screen
    }

    // Get a list of all owned weapon IDs
    public List<int> OwnedWeapons => ownedWeapons;
}
