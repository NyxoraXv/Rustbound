using UnityEngine;

public class MovementParallax : MonoBehaviour
{
    public float parallaxAmount = 20f;   // Control how much the HUD moves when the player moves
    public float rotationAmount = 5f;    // Control how much the HUD rotates
    public float smoothSpeed = 5f;       // How smoothly the HUD will follow the movement and rotation

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Store the initial position and rotation of the HUD/UI element
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        targetPosition = initialPosition;
        targetRotation = initialRotation;
    }

    void Update()
    {
        // Check player input for WASD movement or arrow keys
        float moveX = Input.GetAxis("Horizontal");  // 'A' and 'D' or Left/Right arrow keys
        float moveY = Input.GetAxis("Vertical");    // 'W' and 'S' or Up/Down arrow keys

        // Calculate the target position by moving slightly in the opposite direction of player movement
        targetPosition = new Vector3(
            initialPosition.x - moveX * parallaxAmount,
            initialPosition.y - moveY * parallaxAmount,
            initialPosition.z
        );

        // Calculate the target rotation based on player movement, tilting the HUD/UI
        targetRotation = Quaternion.Euler(
            initialRotation.eulerAngles.x + moveY * rotationAmount,   // Tilt up/down based on vertical movement
            initialRotation.eulerAngles.y,                           // Keep Y-axis rotation fixed
            initialRotation.eulerAngles.z + moveX * rotationAmount    // Tilt left/right based on horizontal movement
        );

        // Smoothly move the HUD/UI element towards the target position and rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
