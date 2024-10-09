using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image weaponImage, decoratorImage;
    [SerializeField] private GameObject statsLine, BuyButton;
    [SerializeField] private Vector3 hoverOffset = new Vector3(0f, 10f, 0f); // Set your desired offset here
    [SerializeField] private float buttonHoverScale = 1.1f; // Scale up value for hover effect
    [SerializeField] private float buttonClickScale = 0.9f; // Scale down value for click effect
    [SerializeField] private float duration = 0.3f; // Duration for the animations

    private Vector3 originalPosition; // Store the original position
    private Button buyButtonComponent;

    private void Start()
    {
        // Store the initial position of the weaponImage
        originalPosition = weaponImage.rectTransform.localPosition;

        // Add the click listener to the BuyButton
        buyButtonComponent = BuyButton.GetComponent<Button>();
        buyButtonComponent.onClick.AddListener(OnBuyButtonClick);
    }

    // This method will be called when the pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();  // Trigger hover effect
    }

    // This method will be called when the pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        OffHover();  // Trigger off-hover effect
    }

    public void OnHover()
    {
        // Scale up and move the weaponImage with the specified offset
        weaponImage.rectTransform.DOScale(1.1f, 0.5f); // Scale up
        weaponImage.rectTransform.DOLocalMove(originalPosition + hoverOffset, 0.5f); // Move to the original position plus offset
        statsLine.transform.DOScale(0.9f, 0.5f);
    }

    public void OffHover()
    {
        // Scale down and return to the original position
        weaponImage.rectTransform.DOScale(1f, 0.5f); // Scale down effect
        weaponImage.rectTransform.DOLocalMove(originalPosition, 0.5f); // Return to original position
        statsLine.transform.DOScale(1f, 0.5f);
    }

    // Method for BuyButton hover effect
    public void OnBuyButtonHoverEnter()
    {
        // Scale up the BuyButton on hover
        BuyButton.transform.DOScale(1.1f, 0.4f);
    }

    public void OnBuyButtonHoverExit()
    {
        // Scale back to original size when not hovering
        BuyButton.transform.DOScale(1f, duration);
    }

    // Method for BuyButton click effect
    private void OnBuyButtonClick()
    {
        // Scale down and then scale up for a click bounce effect
        BuyButton.transform.DOScale(buttonClickScale, duration / 2).OnComplete(() =>
        {
            BuyButton.transform.DOScale(1f, duration / 2);
        });
    }
}
