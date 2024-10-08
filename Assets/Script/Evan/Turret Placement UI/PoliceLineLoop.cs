using UnityEngine;
using DG.Tweening;

public class MoveUIElementWithRotation : MonoBehaviour
{
    public RectTransform uiElement;   // The RectTransform of the UI element
    public RectTransform parent;      // The parent RectTransform for the instantiation
    public float moveDistance;        // Total distance to move the UI element upwards
    public float midDistance;         // Mid-point distance where the next image will instantiate
    public float moveDuration = 2f;   // Duration of the move
    public Vector3 startPoint;

    private void Start()
    {
        // Start moving the UI element upwards
        MoveUpwards();
    }

    private void MoveUpwards()
    {
        // Define the direction (upwards)
        Vector2 direction = new Vector2(0, 1);

        // Calculate the target position for the total move distance
        Vector2 targetPosition = uiElement.anchoredPosition + direction.normalized * moveDistance;

        // Calculate the mid-point position for instantiating a new image
        Vector2 midPointPosition = uiElement.anchoredPosition + direction.normalized * midDistance;

        // Move towards the mid-point first
        uiElement.DOAnchorPos(midPointPosition, moveDuration / 2f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Instantiate the new UI element when the current element reaches the mid-point
                InstantiateNewElement();
                Destroy(gameObject);
                // Continue moving the current element to the target position
                uiElement.DOAnchorPos(targetPosition, moveDuration / 2f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        // Once the move is complete, reset the position or handle any additional logic if needed
                        
                        MoveUpwards(); // Continue the loop if needed
                    });
            });
    }

    private void InstantiateNewElement()
    {
        // Instantiate the new UI element
        Vector3 startPosition = uiElement.localPosition;
        startPosition.y -= moveDistance;  // Place it at the bottom, off-screen or at the start position

        // Create a new instance of the UI element
        GameObject newObject = Instantiate(gameObject, parent);

        // Set the new element's position and rotation
        RectTransform newRectTransform = newObject.GetComponent<RectTransform>();
        newRectTransform.localPosition = startPoint;  // Start from the bottom
        newRectTransform.localRotation = Quaternion.identity;  // Maintain the original rotation

        // Start the movement for the newly instantiated element
        newObject.GetComponent<MoveUIElementWithRotation>().MoveUpwards();
    }
}
