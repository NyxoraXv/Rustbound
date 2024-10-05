using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GPS_Input : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Vector3 lastPosition;
    [SerializeField] private LayerMask placementLayerMask;
    public event Action OnClicked, OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.nearClipPlane;
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayerMask))
        {
            lastPosition = hit.point;
            Debug.Log("Raycast hit at: " + hit.point);
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }

        return lastPosition;
    }
}
