using UnityEngine;

[System.Serializable]
public class TurretData
{
    public string turretName;
    public GameObject turretPrefab;
    public int turretID;
    public int turretDamage;
    public float turretFireRate;
    public float turretRange;
    public int price; // Add price for purchasing
}
