using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if(Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, placementLayermask))
        {
            lastPosition = hit.point;  // Update the last valid position
            return lastPosition;       // Return hit position
        }

        return lastPosition;  // Return the last known valid position if nothing is hit
    }

}
