using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance { get; private set; }

    public TurretDatabase turretDatabase; // Reference this in the Inspector

    private List<int> ownedTurrets = new List<int>(); // Store owned turret IDs

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

    // Method to buy a turret by ID
    public bool BuyTurret(int turretID)
    {
        TurretData turretToBuy = turretDatabase.GetTurretByID(turretID);

        if (turretToBuy != null && !IsTurretOwned(turretID))
        {
            if (CurrencyManager.Instance.SpendCurrency(turretToBuy.price))
            {
                ownedTurrets.Add(turretID);
                Debug.Log("Turret bought: " + turretToBuy.turretName);
                return true;
            }
            else
            {
                Debug.Log("Not enough currency to buy " + turretToBuy.turretName);
                return false;
            }
        }
        else
        {
            Debug.Log("Turret already owned or not found.");
            return false;
        }
    }

    // Check if a turret is owned by ID
    public bool IsTurretOwned(int turretID)
    {
        return ownedTurrets.Contains(turretID);
    }

    // Method to get turret data by ID (useful for placing or displaying stats)
    public TurretData GetTurretByID(int turretID)
    {
        return turretDatabase.GetTurretByID(turretID);
    }
}
