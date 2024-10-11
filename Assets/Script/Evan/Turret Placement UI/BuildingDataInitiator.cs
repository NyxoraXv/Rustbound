using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDataInitiator : MonoBehaviour  
{
    public TextMeshProUGUI nameText; // Reference to the name text component
    public TextMeshProUGUI priceText; // Reference to the price text component
    public Image iconImage; // Reference to the icon image component
    public int id;

    public void Initialize(StructureData structure)
    {
        nameText.text = structure.Name;
        priceText.text = structure.price.ToString();
        iconImage.sprite = structure.Image;
        id = structure.ID;
    }

    public void Initialize(TurretData turret)
    {
        nameText.text = turret.Name;
        priceText.text = turret.price.ToString();
        iconImage.sprite = turret.Image;
        id = turret.ID;
    }

    private void Start()
    {
        // Find the button component and add a click listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Find the PlacementSystem instance and start placement with the given ID
        Debug.Log(id);
        PlacementSystem.Instance.StartPlacement(id);
    }
}