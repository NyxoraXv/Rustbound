using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // DoTween for smooth animations

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }

    [SerializeField] private InputManager inputManager;
    [SerializeField] private ObjectsDatabaseSO objectsDatabase;
    [SerializeField] private Material previewMaterial;   // Material for previewing placement
    [SerializeField] private Material removeMaterial;    // Material for previewing removal
    [SerializeField] private LayerMask objectLayerMask;  // Mask for checking existing objects
    [SerializeField] private float yOffset = 0.5f;       // Offset for y-axis placement

    private GameObject previewObject;
    private ObjectData currentObjectData;
    private bool isPlacing, isRemoving;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Optional: To persist across scenes
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
            UpdatePlacementPreview();
        else if (isRemoving)
            UpdateRemovePreview();
    }

    // Called to start the placement of a new object based on its ID
    public void StartPlacement(int id)
    {
        if (id < 0 || id >= objectsDatabase.objectsData.Count) return;

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

        // Animate preview object in with DoTween for smooth transition
        previewObject.transform.localScale = Vector3.zero;
        previewObject.transform.DOScale(Vector3.one, 0.3f);
    }

    // Called to start removing an object
    public void StartRemove()
    {
        isRemoving = true;
        isPlacing = false;
    }

    // Cancel current action
    private void CancelAction()
    {
        isPlacing = false;
        isRemoving = false;
        if (previewObject != null) Destroy(previewObject);
    }

    // Handle clicking during placement or removal
    private void HandleClick()
    {
        if (isPlacing)
        {
            if (IsPlacementValid())
            {
                PlaceObject();
            }
        }
        else if (isRemoving)
        {
            RemoveObject(currentObjectData.ID);
        }
    }

    // Preview and update object placement
    private void UpdatePlacementPreview()
    {
        Vector3 position = inputManager.GetSelectedMapPosition();
        position.y += yOffset;  // Apply y offset for the preview as well

        if (previewObject != null)
        {
            previewObject.transform.position = position;

            if (IsPlacementValid())
            {
                SetPreviewColor(previewMaterial.color);  // Set valid color
            }
            else
            {
                SetPreviewColor(Color.red);  // Set invalid color
            }
        }
    }

    // Preview removal by highlighting object
    private void UpdateRemovePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, objectLayerMask))
        {
            GameObject target = hit.collider.gameObject;
            ApplyPreviewMaterial(target, removeMaterial);  // Highlight the object to be removed
        }
    }

    // Check if the current placement is valid
    private bool IsPlacementValid()
    {
        // Check for collisions with objects tagged as "Player", "Building", or "Turret"
        Collider[] colliders = Physics.OverlapSphere(previewObject.transform.position, 0.5f, objectLayerMask);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Building") || collider.CompareTag("Turret"))
            {
                Debug.Log("Invalid placement: Colliding with " + collider.tag);  // Output debug message
                return false;  // Invalid if it collides with Player, Building, or Turret
            }
        }

        // If no invalid collisions are detected, placement is valid
        return true;
    }

    // Place the object in the world
    // Place the object in the world
    private void PlaceObject()
    {
        Vector3 position = previewObject.transform.position;

        // Instantiate the placed object
        GameObject placedObject = Instantiate(currentObjectData.Prefab, position, previewObject.transform.rotation);
        currentObjectData.currentSpawnedTurret++;

        // Activate all colliders in the placed object
        ActivateColliders(placedObject.transform);

        // Destroy the preview object
        Destroy(previewObject);
        isPlacing = false;
    }

    // Activate all colliders in the given transform and its children
    private void ActivateColliders(Transform objTransform)
    {
        // Get all colliders in the transform and its children
        Collider[] colliders = objTransform.GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            collider.enabled = true; // Enable each collider
        }
    }


    // Remove an object from the world
    public void RemoveObject(int turretID)
    {
        ObjectData selectedObjectData = objectsDatabase.objectsData[turretID];
        selectedObjectData.currentSpawnedTurret--;

        if (selectedObjectData.currentSpawnedTurret > 0)
        {
            selectedObjectData.currentSpawnedTurret--;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, objectLayerMask))
        {
            Destroy(hit.collider.gameObject);
            isRemoving = false;
        }
    }

    // Apply the preview material to an object
    private void ApplyPreviewMaterial(GameObject obj, Material mat)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = mat;
        }
    }

    // Set the preview object's color based on valid/invalid placement
    private void SetPreviewColor(Color color)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }
}
