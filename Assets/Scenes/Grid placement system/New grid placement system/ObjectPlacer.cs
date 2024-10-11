using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new List<GameObject>();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        position.y = position.y + 0.5f;
        GameObject newObject = Instantiate(prefab);
        //diaktifkan
        Collider[] colliders = newObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        newObject.transform.position = position;
        placedGameObjects.Add(newObject);

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
