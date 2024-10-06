using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Database/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponData> weapons;

    // Method to get weapon by its ID
    public WeaponData GetWeaponByID(int weaponID)
    {
        return weapons.Find(weapon => weapon.weaponID == weaponID);
    }
}
