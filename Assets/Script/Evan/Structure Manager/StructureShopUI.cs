using System.Collections.Generic;
using UnityEngine;

public class StructureShopUI : MonoBehaviour
{
    [SerializeField] private GameObject structureContainerPrefab; // The prefab with WeaponContainerInitiator
    [SerializeField] private Transform structureContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 posAddition;
    private StructureDatabase structureDatabase;    // The database of weapons

    private void OnEnable()
    {
        structureDatabase = StructureManager.Instance.structureDatabase;
        PopulateConstructionWindow();
    }

    private void PopulateConstructionWindow()
    {
        List<StructureData> allTurret = structureDatabase.GetAllStructure();
        int i = 0;
        foreach (StructureData structure in allTurret)
        {
            // Instantiate the weapon prefab and set its parent
            GameObject newStructureContainer = Instantiate(structureContainerPrefab, structureContainerParent);
            newStructureContainer.GetComponent<RectTransform>().anchoredPosition = (Vector2.zero+(posAddition*i));
            i += 1;
            // Get the WeaponContainerInitiator component and initialize it
            StructureContainerInitiator initiator = newStructureContainer.GetComponent<StructureContainerInitiator>();
            initiator.Initialize(structure); // Pass the current weapon data to the initializer
        }
    }
}
