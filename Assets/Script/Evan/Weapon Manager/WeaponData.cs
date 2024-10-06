using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public GameObject weaponPrefab;
    public Sprite weaponImage;
    public int weaponID;
    public int weaponDamage;
    public float weaponRateOfFire;
    public float weaponAccuracy;
    public int price; // Add a price if you need it for purchasing.

    // You can add more fields if needed, like description, ammo capacity, etc.
}
