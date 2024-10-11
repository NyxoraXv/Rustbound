using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIInformation : MonoBehaviour
{
    public static UIInformation instance;

    public TextMeshProUGUI messageText; // Assign via inspector
    public GameObject popupPanel; // UI panel containing the popup
    public CanvasGroup canvasGroup; // The CanvasGroup component attached to this GameObject
    public Image overlayImage; // Overlay image to fade separately
    public GameObject backgroundImage; // Background image that animates differently

    public float animationDuration = 0.5f; // Default animation duration
    public float scaleInSize = 1.2f; // Scale size for popup effect

    public enum PopupMessageType
    {
        NotEnoughBalance,
        MaxTurretPlacement,
        InCombat
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Ensure everything is dissolved without animation at the start
        InstantDissolve();
    }

    // Call this method to show the appropriate message with animation
    // Call this method to show the appropriate message with animation
    public void ShowPopup(PopupMessageType messageType, float dissolveTime = 0.5f)
    {
        // Set the message
        string message = GetMessageText(messageType);
        messageText.text = message;

        // Enable the popup panel and child objects
        popupPanel.SetActive(true);
        EnablePopup();

        // Animate the canvas group alpha and scaling for the entire panel
        canvasGroup.DOFade(1, dissolveTime).SetEase(Ease.OutQuad);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Reset the scale for the popup and animate the scaling
        popupPanel.transform.localScale = Vector3.zero; // Start hidden
        popupPanel.transform.DOScale(scaleInSize, dissolveTime)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                popupPanel.transform.DOScale(1f, 0.2f); // Scale back to normal size
            });

        // Animate the overlay separately by adjusting its alpha
        overlayImage.DOFade(230f / 255f, dissolveTime).SetEase(Ease.OutQuad);

        // Automatically dissolve after 1 second
        Invoke(nameof(HidePopup), 1f);
    }


    // Hide the popup with a fade out and scale down animation
    public void HidePopup()
    {
        // Animate the canvas group alpha to fade out the entire panel
        canvasGroup.DOFade(0, animationDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                DisablePopup(); // Disable child objects after fade out
            });

        // Scale down the popup panel during the hide animation
        popupPanel.transform.DOScale(0, animationDuration).SetEase(Ease.InBack);

        // Fade out the overlay
        overlayImage.DOFade(0, animationDuration).SetEase(Ease.InQuad);
    }

    // Enable the popup and its children
    private void EnablePopup()
    {
        // Enable all children inside the popupPanel
        foreach (Transform child in popupPanel.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Disable the popup and its children
    private void DisablePopup()
    {
        // Disable all children inside the popupPanel
        foreach (Transform child in popupPanel.transform)
        {
            child.gameObject.SetActive(false);
        }

        popupPanel.SetActive(false);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // Instantly dissolve all elements (no animation) at the start using CanvasGroup
    private void InstantDissolve()
    {
        popupPanel.transform.localScale = Vector3.zero; // Hide popup panel
        canvasGroup.alpha = 0; // Make everything invisible instantly using CanvasGroup
        canvasGroup.interactable = false; // Disable interactions
        canvasGroup.blocksRaycasts = false; // Disable raycast blocking

        // Set overlay alpha to 0 instantly
        overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, 0);
    }

    // Get the appropriate message based on the enum type
    private string GetMessageText(PopupMessageType messageType)
    {
        switch (messageType)
        {
            case PopupMessageType.NotEnoughBalance:
                return "You clearly broke, get some money!";
            case PopupMessageType.MaxTurretPlacement:
                return "You have reached the maximum number of turrets allowed.";
            case PopupMessageType.InCombat:
                return "You are still in combat. Please try again after the round is complete.";
            default:
                return "Unknown error.";
        }
    }
}
