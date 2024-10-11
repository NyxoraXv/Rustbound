using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public int maxSpawnTurret { get; private set; }
    public int currentSpawnedTurret { get; set; } = 0;  // New field to track spawned turrets
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
