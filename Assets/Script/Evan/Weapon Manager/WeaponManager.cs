using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public WeaponDatabase weaponDatabase; // Assign this in the Inspector.
    public bool isPrimary;

    // Equipped weapon slots (2 slots)
    private WeaponData equippedWeaponSlot1;
    private WeaponData equippedWeaponSlot2;

    [SerializeField] private InventoryUIAnimator UIanim;

    public Image weaponSlot1Image; // UI Image for equipped weapon slot 1
    public Image weaponSlot2Image; // UI Image for equipped weapon slot 2

    private List<int> ownedWeapons = new List<int>(); // Store owned weapon IDs

    public bool isSelectingWeapon = false; // Indicates whether the player is in weapon selection mode
    private int currentSelectionSlot; // Track the currently selected slot for equipping

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

    // Start weapon selection mode from slot button press
    public void BeginSelectWeapon(int slot)
    {
        isSelectingWeapon = true;
        currentSelectionSlot = slot; // Set the current selection slot
        if(slot == 1)
        {
            UIanim.TriggerPrimarySlot(true);
        }
        else
        {
            UIanim.TriggerSecondarySlot(true);
        }
        Debug.Log("Entering weapon selection mode for Slot " + slot);
        // Additional UI setup can be done here, such as opening a weapon selection screen
        // Here you might want to show available weapons for selection
    }

    // Handle weapon selection in UI
    public void OnWeaponSelected(int weaponID)
    {
        // Attempt to equip the selected weapon in the current selection slot
        EquipWeapon(weaponID, currentSelectionSlot); // Use the current selection slot instead of checking for empty slots
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
                if (equippedWeaponSlot1 != null)
                {
                    Debug.Log("Replacing weapon in Slot 1: " + equippedWeaponSlot1.weaponName);
                }
                isPrimary = true;
                equippedWeaponSlot1 = weaponToEquip; // Equip the new weapon
                weaponSlot1Image.sprite = weaponToEquip.weaponImage; // Update UI for slot 1
                Debug.Log("Weapon equipped in Slot 1: " + equippedWeaponSlot1.weaponName);
                
            }
            else if (slot == 2)
            {
                if (equippedWeaponSlot2 != null)
                {
                    Debug.Log("Replacing weapon in Slot 2: " + equippedWeaponSlot2.weaponName);
                }

                isPrimary = false;
                equippedWeaponSlot2 = weaponToEquip; // Equip the new weapon
                weaponSlot2Image.sprite = weaponToEquip.weaponImage; // Update UI for slot 2
                Debug.Log("Weapon equipped in Slot 2: " + equippedWeaponSlot2.weaponName);
            }

            ExitSelectWeapon(); // Exit selection mode after equipping

        }
        else
        {
            Debug.Log("Weapon not owned.");
        }
    }

    // Exit weapon selection mode
    public void ExitSelectWeapon()
    {
        isSelectingWeapon = false;
        UIanim.TriggerPrimarySlot(false);
        UIanim.TriggerSecondarySlot(false);
        Debug.Log("Exiting weapon selection mode.");
        // Additional UI teardown can be done here, such as closing the weapon selection screen
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

    // Get a list of all owned weapon IDs
    public List<int> OwnedWeapons => ownedWeapons;
}
