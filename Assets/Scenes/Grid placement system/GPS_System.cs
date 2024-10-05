using DG.Tweening;
using System;
using UnityEngine;

public class GPS_System : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, CellIndicator;
    [SerializeField] private GPS_Input input;
    [SerializeField] private Grid grid;
    [SerializeField] private GPS_db database;
    [SerializeField] private int selectedObjectIndex = -1;
    [SerializeField] private GameObject gridVisualization;

    private void Start()
    {
        stopPlacement();
    }

    private void stopPlacement()
    {
        gridVisualization.SetActive(false);
        CellIndicator.SetActive(false);
        input.OnClicked -= PlaceStructure;
        input.OnExit -= stopPlacement;
    }

    public void startPlacement(int id)
    {
        stopPlacement();
        selectedObjectIndex = database.objectData.FindIndex(data => data.id == id);
        if (selectedObjectIndex < 0)
        {
            Debug.Log("no id found");
            return;
        }
        gridVisualization.SetActive(true);
        CellIndicator.SetActive(true);
        input.OnClicked += PlaceStructure;
        input.OnExit  += stopPlacement;
    }

    private void PlaceStructure()
    {
        if(input.IsPointerOverUI()) 
        {
            Vector3 mousePosition = input.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            GameObject obj = Instantiate(database.objectData[selectedObjectIndex].prefab);
            obj.transform.position = grid.CellToWorld(gridPosition);
        }
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        Vector3 mousePosition = input.GetSelectedMapPosition();
        mouseIndicator.transform.position = mousePosition;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        CellIndicator.transform.DOMove(new Vector3(grid.CellToWorld(gridPosition).x, 0, grid.CellToWorld(gridPosition).z), 1f);
    }
}
