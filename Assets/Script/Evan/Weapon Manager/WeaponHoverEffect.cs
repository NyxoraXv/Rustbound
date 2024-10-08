using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image weaponImage, decoratorImage;
    [SerializeField] private GameObject statsLine;
    [SerializeField] private Vector3 hoverOffset = new Vector3(0f, 10f, 0f); // Set your desired offset here

    private Vector3 originalPosition; // Store the original position

    private void Start()
    {
        // Store the initial position of the weaponImage
        originalPosition = weaponImage.rectTransform.localPosition;
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
        statsLine.transform.DOLocalMoveX(originalPosition.z+hoverOffset.z, 0.5f);
    }

    public void OffHover()
    {
        // Scale down and return to the original position
        weaponImage.rectTransform.DOScale(1f, 0.5f); // Scale down effect
        weaponImage.rectTransform.DOLocalMove(originalPosition, 0.5f); // Return to original position
        statsLine.transform.DOLocalMoveX(originalPosition.z, 0.5f);
    }
}
