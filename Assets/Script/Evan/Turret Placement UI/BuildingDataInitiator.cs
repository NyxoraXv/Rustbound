using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDataInitiator : MonoBehaviour  
{
    public TextMeshProUGUI nameText; // Reference to the name text component
    public TextMeshProUGUI priceText; // Reference to the price text component
    public Image iconImage; // Reference to the icon image component

    public void Initialize(StructureData structure)
    {
        nameText.text = structure.Name;
        priceText.text = structure.price.ToString();
        iconImage.sprite = structure.Image;
    }

    public void Initialize(TurretData turret)
    {
        nameText.text = turret.Name;
        priceText.text = turret.price.ToString();
        iconImage.sprite = turret.Image;
    }
}