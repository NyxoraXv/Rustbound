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
    [Range(0, 1.5f)]
    public float weaponAccuracy;
    public int price; // Price for purchasing
    public float reloadTime;     // Time to reload the weapon
    public int magazineSize;     // Number of bullets per magazine
    public int currentAmmo;      // Current bullets in the magazine
    public float shootForce;     // Bullet force
    public ShootingType shootingType; // Type of shooting (Normal, Spread, Missile)

    // You can add more fields if needed, like description, etc.
}
