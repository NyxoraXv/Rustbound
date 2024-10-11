using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Net;  // DoTween for smooth animations

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }

    [SerializeField] private InputManager inputManager;
    [SerializeField] private ObjectsDatabaseSO objectsDatabase;
    [SerializeField] private Material previewMaterial;   // Material for previewing placement
    [SerializeField] private Material removeMaterial;    // Material for previewing removal
    [SerializeField] private LayerMask objectLayerMask;  // Mask for checking existing objects
    [SerializeField] private float yOffset = 0.5f;       // Offset for y-axis placement
    [SerializeField] private KeyCode rotateKey = KeyCode.R;  // Key to rotate preview object
    [SerializeField] private Vector3 rotationAmount = new Vector3(0, 45, 0); // Rotation angle for preview
    private float fixedYPosition = 0f;

    private GameObject previewObject;
    private ObjectData currentObjectData;
    private bool isPlacing, isRemoving;
    private List<MonoBehaviour> previewScripts = new List<MonoBehaviour>(); // Store all disabled scripts

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        foreach (var objectData in objectsDatabase.objectsData)
        {
            objectData.currentSpawnedTurret = 0; // Set to 0 at the start
        }
    }

    private void OnEnable()
    {
        inputManager.OnClicked += HandleClick;
        inputManager.OnExit += CancelAction;
    }

    private void OnDisable()
    {
        inputManager.OnClicked -= HandleClick;
        inputManager.OnExit -= CancelAction;
    }

    private void Update()
    {
        if (isPlacing)
        {
            UpdatePlacementPreview();

            // Rotate the preview object if the rotate key is pressed
            if (Input.GetKeyDown(rotateKey))
            {
                RotatePreviewObject();
            }

            // Animate the preview's movement (e.g., floating up and down)
            AnimatePreviewMovement();
        }
        else if (isRemoving)
        {
            UpdateRemovePreview();
        }
    }

    public void StartPlacement(int id)
    {
        if (id < 0 || id >= objectsDatabase.objectsData.Count) return;

        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        currentObjectData = objectsDatabase.objectsData[id];
        if (currentObjectData.currentSpawnedTurret >= currentObjectData.maxSpawnTurret)
        {
            Debug.LogWarning("Max turrets placed.");
            return;
        }

        isPlacing = true;
        isRemoving = false;

        // Create the preview object
        previewObject = Instantiate(currentObjectData.Prefab);
        ApplyPreviewMaterial(previewObject, previewMaterial);

        // Freeze the Y position of the preview object
        fixedYPosition = previewObject.transform.position.y;

        // Disable all scripts in the preview object
        DisableScriptsInPreview(previewObject);

        // Animate preview object in with DoTween for smooth transition
        previewObject.transform.localScale = Vector3.zero;
        previewObject.transform.DOScale(Vector3.one, 0.3f);
    }

    // Disable all MonoBehaviour scripts in the preview object
    private void DisableScriptsInPreview(GameObject obj)
    {
        previewScripts.Clear(); // Clear the list in case of new preview
        MonoBehaviour[] scripts = obj.GetComponentsInChildren<MonoBehaviour>();

        foreach (var script in scripts)
        {
            if (script.enabled)
            {
                script.enabled = false; // Disable the script
                previewScripts.Add(script); // Add to the list for later re-enabling
            }
        }
    }

    // Called to start removing an object
    public void StartRemove()
    {
        isRemoving = true;
        isPlacing = false;

        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }

    // Cancel current action
    private void CancelAction()
    {
        isPlacing = false;
        isRemoving = false;

        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }

    // Handle clicking during placement or removal
    private void HandleClick()
    {
        if (isPlacing)
        {
            if (IsPlacementValid())
            {
                if (CurrencyManager.Instance.SpendCurrency((TurretManager.Instance.turretDatabase.GetTurretByID(currentObjectData.ID).price)))
                {
                    StartCoroutine(PlaceObjectWithDelay(0f));  // Add delay before placing (e.g., 0.5 seconds)
                }
                else
                {
                    Debug.LogWarning("Failed to spend currency.");
                }
            }
        }
        else if (isRemoving)
        {
            RemoveObject(currentObjectData.ID);
        }
    }

    // Coroutine to handle delayed placement
    private IEnumerator PlaceObjectWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay

        PlaceObject();  // Call the original PlaceObject method
    }

    // Preview and update object placement
    private void UpdatePlacementPreview()
    {
        Vector3 position = inputManager.GetSelectedMapPosition();

        // Maintain the fixed y position
        position.y = fixedYPosition;  // Set y position to the fixed value

        if (previewObject != null)
        {
            previewObject.transform.position = position;

            if (IsPlacementValid())
            {
                SetPreviewColor(Color.gray);  // Set valid color
            }
            else
            {
                SetPreviewColor(Color.red);  // Set invalid color
            }
        }
    }

    // Update remove preview
    private void UpdateRemovePreview()
    {
        Vector3 position = inputManager.GetSelectedMapPosition();

        // Maintain the fixed y position
        position.y = fixedYPosition;  // Apply the fixed Y position

        if (previewObject != null)
        {
            previewObject.transform.position = position;

            // Check if the object under the cursor can be removed
            if (IsRemovalValid())
            {
                SetPreviewColor(removeMaterial.color);  // Set valid removal color
            }
            else
            {
                SetPreviewColor(Color.red);  // Set invalid removal color
            }
        }
    }


    // Check if the current removal is valid
    private bool IsRemovalValid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, objectLayerMask))
        {
            // You can check if the hit object is removable (has a specific tag or component)
            return hit.collider.CompareTag("Turret") || hit.collider.CompareTag("Building");
        }
        return false;
    }

    // Animate preview movement (e.g., floating effect)
    private void AnimatePreviewMovement()
    {
        if (previewObject != null)
        {
            // Example: Rotate and float up and down

        }
    }

    // Rotate the preview object by the specified angle
    // Rotate the preview object by 45 degrees using DoTween
    private void RotatePreviewObject()
    {
        if (previewObject != null)
        {
            // Calculate the new rotation
            Vector3 newRotation = previewObject.transform.rotation.eulerAngles + rotationAmount;

            // Use DoTween to rotate to the new angle smoothly
            previewObject.transform.DORotate(newRotation, 0.3f)  // Duration for the rotation
                .SetEase(Ease.OutQuad);  // Choose an easing function for the animation
        }
    }


    // Place the object in the world
    private void PlaceObject()
    {
        Vector3 position = previewObject.transform.position;
        Quaternion rotation = previewObject.transform.rotation;

        // Instantiate the placed object
        GameObject placedObject = Instantiate(currentObjectData.Prefab, position, rotation);
        currentObjectData.currentSpawnedTurret++;

        // Re-enable all scripts in the placed object
        ReenableScripts(placedObject);

        // Activate all colliders in the placed object
        ActivateColliders(placedObject.transform);

        Destroy(previewObject);  // Destroy the preview
        isPlacing = false;
    }

    // Re-enable all scripts after the object is placed
    private void ReenableScripts(GameObject placedObject)
    {
        foreach (var script in previewScripts)
        {
            script.enabled = true;  // Re-enable each script
        }
    }

    // Check if the current placement is valid, ignoring its own colliders
    private bool IsPlacementValid()
    {
       
    if (!CurrencyManager.Instance.HasEnoughCurrency((TurretManager.Instance.turretDatabase.GetTurretByID(currentObjectData.ID).price)))
        {
            Debug.LogWarning("Not enough currency to place the object.");
            return false;
        }
        Collider[] colliders = Physics.OverlapSphere(previewObject.transform.position, 0.5f, objectLayerMask);

        foreach (var collider in colliders)
        {
            if (collider.gameObject == previewObject || collider.transform.IsChildOf(previewObject.transform))
            {
                continue;
            }

            if (collider.CompareTag("Player") || collider.CompareTag("Building") || collider.CompareTag("Turret"))
            {
                Debug.Log("Invalid placement: Colliding with " + collider.tag);
                return false;
            }
        }

        return true;
    }

    // Remove an object by its turret ID
    public void RemoveObject(int turretID)
    {
        ObjectData selectedObjectData = objectsDatabase.objectsData[turretID];

        if (selectedObjectData.currentSpawnedTurret > 0)
        {
            // Logic to find and destroy the corresponding turret game object
            // Implement your logic here to find and remove the specific turret
            selectedObjectData.currentSpawnedTurret--;
        }

        Destroy(previewObject);  // Destroy the preview if it exists
    }

    // Apply a material to the preview object
    private void ApplyPreviewMaterial(GameObject obj, Material material)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            renderer.material = material;  // Set the preview material
        }
    }

    // Set the color of the preview object
    private void SetPreviewColor(Color color)
    {
        if (previewObject != null)
        {
            Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                renderer.material.color = color;  // Set the color
            }
        }
    }

    // Activate all colliders in the object hierarchy
    private void ActivateColliders(Transform parent)
    {
        Collider[] colliders = parent.GetComponentsInChildren<Collider>();

        foreach (var collider in colliders)
        {
            collider.enabled = true;  // Enable each collider
        }
    }
}
