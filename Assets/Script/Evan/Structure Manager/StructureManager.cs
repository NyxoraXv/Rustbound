using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureManager : MonoBehaviour
{
    public static StructureManager Instance { get; private set; }

    public StructureDatabase structureDatabase; // Assign this in the Inspector.
    private List<int> ownedStructure = new List<int>(); // Store owned weapon IDs

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
    public bool buyStructure(int structureID)
    {
        StructureData turretToBuy = structureDatabase.GetStructureByID(structureID);

        if (turretToBuy != null && !isStructureOwned(structureID))
        {
            if (CurrencyManager.Instance.SpendCurrency(turretToBuy.price))
            {
                ownedStructure.Add(structureID); // Add weapon ID to the owned list
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
    public bool isStructureOwned(int ID)
    {
        return ownedStructure.Contains(ID);
    }

    public List<int> OwnedStructure => ownedStructure;
}
