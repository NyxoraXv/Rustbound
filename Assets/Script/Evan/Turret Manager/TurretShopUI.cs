using System.Collections.Generic;
using UnityEngine;

public class TurretShopUI : MonoBehaviour
{
    [SerializeField] private GameObject turretContainerPrefab; // The prefab with WeaponContainerInitiator
    [SerializeField] private Transform turretContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 posAddition;
    private TurretDatabase turretData;    // The database of weapons
    

    private void OnEnable()
    {
        turretData = TurretManager.Instance.turretDatabase;
        PopulateConstructionWindow();
    }

    private void PopulateConstructionWindow()
    {
        List<TurretData> allTurret = turretData.GetAllTurret();
        int i = 0;
        foreach (TurretData turret in allTurret)
        {
            // Instantiate the weapon prefab and set its parent
            GameObject newTurretContianer = Instantiate(turretContainerPrefab, turretContainerParent);
            newTurretContianer.GetComponent<RectTransform>().anchoredPosition = (Vector2.zero+(posAddition*i));
            i += 1;
            // Get the WeaponContainerInitiator component and initialize it
            TurretContainerInitiator initiator = newTurretContianer.GetComponent<TurretContainerInitiator>();
            initiator.Initialize(turret); // Pass the current weapon data to the initializer
        }
    }
}
