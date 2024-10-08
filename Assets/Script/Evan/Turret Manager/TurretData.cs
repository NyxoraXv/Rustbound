using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TurretData
{
    public string Name;
    public GameObject Prefab;
    public Sprite Image;
    public int ID;
    public int Damage;
    public float RateOfFire;
    public float Accuracy;
    public int price; // Add a price if you need it for purchasing.

    // You can add more fields if needed, like description, ammo capacity, etc.
}
