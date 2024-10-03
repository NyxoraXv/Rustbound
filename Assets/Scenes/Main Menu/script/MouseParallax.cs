using UnityEngine;

public class MouseParallax : MonoBehaviour
{
    public float parallaxAmount = 0.05f; // Control the intensity of the effect

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial camera position
        initialPosition = transform.position;
    }

    void Update()
    {
        // Get mouse position relative to screen width/height, normalized to -1 and 1
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Apply a small opposite movement to the camera based on mouse position
        Vector3 targetPosition = new Vector3(
            initialPosition.x ,
            initialPosition.y - mouseY * parallaxAmount,
            initialPosition.z - mouseX * parallaxAmount
        );

        // Smoothly move the camera to the new position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2);
    }
}
