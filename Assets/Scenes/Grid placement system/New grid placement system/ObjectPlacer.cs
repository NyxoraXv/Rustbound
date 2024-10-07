using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new List<GameObject>();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        placedGameObjects.Add(newObject);

        // Log information about the instantiated object and its children
        Debug.Log($"Instantiated object: {newObject.name}");

        foreach (Transform child in newObject.transform)
        {
            Debug.Log($"Child object: {child.name}, active: {child.gameObject.activeSelf}");
        }

        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (gameObjectIndex < 0 || gameObjectIndex >= placedGameObjects.Count)
            return;

        GameObject objToRemove = placedGameObjects[gameObjectIndex];
        if (objToRemove != null)
        {
            Destroy(objToRemove);
            placedGameObjects[gameObjectIndex] = null;
        }
    }
}
