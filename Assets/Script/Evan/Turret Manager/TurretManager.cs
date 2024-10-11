using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance { get; private set; }

    public TurretDatabase turretDatabase; // Assign this in the Inspector.
    private List<int> ownedTurret = new List<int>(); // Store owned weapon IDs

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    // Method to buy a weapon by ID
    public bool buyTurret(int TurretID)
    {
        TurretData turretToBuy = turretDatabase.GetTurretByID(TurretID);

        if (turretToBuy != null && !IsTurretOwned(TurretID))
        {
            if (CurrencyManager.Instance.SpendCurrency(turretToBuy.price))
            {
                ownedTurret.Add(TurretID); // Add weapon ID to the owned list
                Debug.Log("Weapon bought: " + turretToBuy.Name);
                return true;
            }
            else
            {
                Debug.Log("Not enough currency to buy " + turretToBuy.Name);
                return false;
            }
        }
        else
        {
            Debug.Log("Weapon already owned or not found.");
            return false;
        }
    }

    // Check if a weapon is owned by ID
    public bool IsTurretOwned(int turretID)
    {
        return ownedTurret.Contains(turretID);
    }

    public List<int> OwnedTurrets => ownedTurret;
}
