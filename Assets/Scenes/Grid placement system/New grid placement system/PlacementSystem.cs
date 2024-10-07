using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioClip correctPlacementClip, wrongPlacementClip;
    [SerializeField]
    private AudioSource source;

    private GridData floorData, furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    [SerializeField]
    private SoundFeedback soundFeedback;

    private void Start()
    {
        gridVisualization.SetActive(false);
        floorData = new();
        furnitureData = new();

        foreach (ObjectData objData in database.objectsData)
        {
            objData.currentSpawnedTurret = 0; // Reset the count to 0
        }
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);

        buildingState = new PlacementState(ID,
                                        grid,
                                        preview,
                                        database,
                                        floorData,
                                        furnitureData,
                                        objectPlacer,
                                        soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true) ;
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // Get the currently selected object data from the database
        int selectedObjectID = ((PlacementState)buildingState).ID;
        ObjectData selectedObjectData = database.objectsData[selectedObjectID];

        // Debug log to check the placement attempt
        Debug.Log($"Attempting to place turret. Current: {selectedObjectData.currentSpawnedTurret}, Max: {selectedObjectData.maxSpawnTurret}");

        // Check if the max number of turrets is reached
        if (selectedObjectData.currentSpawnedTurret >= selectedObjectData.maxSpawnTurret)
        {
            Debug.Log("Max turrets reached. Cannot place any more turrets.");
            source.PlayOneShot(wrongPlacementClip); // Play feedback for invalid placement
            return;
        }

        // Check if the grid position is occupied
        if (IsGridPositionOccupied(gridPosition))
        {
            Debug.Log("Grid position is already occupied. Cannot place turret.");
            source.PlayOneShot(wrongPlacementClip); // Play feedback for invalid placement
            return;
        }

        // Place the turret if the limit has not been reached
        buildingState.OnAction(gridPosition);

        // Increment the count of currently spawned turrets
        selectedObjectData.currentSpawnedTurret++;
        Debug.Log($"Turret placed. New count: {selectedObjectData.currentSpawnedTurret}");
        source.PlayOneShot(correctPlacementClip); // Play feedback for valid placement
    }

    // Method to check if the grid position is occupied
    private bool IsGridPositionOccupied(Vector3Int gridPosition)
    {
        // Check the grid data for existing objects at the grid position
        return floorData.IsOccupied(gridPosition) || furnitureData.IsOccupied(gridPosition);
    }


    public void RemoveTurret(int turretID)
    {
        ObjectData selectedObjectData = database.objectsData[turretID];
        selectedObjectData.currentSpawnedTurret--;
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? 
    //        floorData : 
    //        furnitureData;

    //    return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    private void StopPlacement()
    {
        soundFeedback.PlaySound(SoundType.Click);
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Debug.Log(mousePosition);
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Debug.Log(gridPosition);
        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
        
    }
}
