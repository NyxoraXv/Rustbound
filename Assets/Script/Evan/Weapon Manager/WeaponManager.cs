using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public WeaponDatabase weaponDatabase; // Assign this in the Inspector.

    // Equipped weapon slots (2 slots)
    private WeaponData equippedWeaponSlot1;
    private WeaponData equippedWeaponSlot2;

    private List<int> ownedWeapons = new List<int>(); // Store owned weapon IDs

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
                ownedWeapons.Add(weaponID);
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

            if (slot == 1)
            {
                equippedWeaponSlot1 = weaponToEquip;
                Debug.Log("Weapon equipped in Slot 1: " + equippedWeaponSlot1.weaponName);
            }
            else if (slot == 2)
            {
                equippedWeaponSlot2 = weaponToEquip;
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
            Debug.Log("Unequipped weapon from Slot 1: " + equippedWeaponSlot1.weaponName);
            equippedWeaponSlot1 = null;
        }
        else if (slot == 2)
        {
            Debug.Log("Unequipped weapon from Slot 2: " + equippedWeaponSlot2.weaponName);
            equippedWeaponSlot2 = null;
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
}
