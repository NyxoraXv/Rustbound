using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Database/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponData> weapons;

    // Method to get a weapon by its ID
    public WeaponData GetWeaponByID(int weaponID)
    {
        return weapons.Find(weapon => weapon.weaponID == weaponID);
    }

    public GameObject[] GetAllWeaponObject()
    {
        return weapons.Select(weapon => weapon.weaponPrefab).ToArray();
    }
    // Method to get all weapons
    public List<WeaponData> GetAllWeapons()
    {
        return weapons;
    }


}
