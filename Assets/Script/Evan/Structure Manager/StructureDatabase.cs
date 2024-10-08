using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureDatabase", menuName = "Database/StructureDatabase")]
public class StructureDatabase : ScriptableObject
{
    public List<StructureData> structure;

    public StructureData GetStructureByID(int ID)
    {
        return structure.Find(structure => structure.ID == ID);
    }

    public List<StructureData> GetAllStructure()
    {
        return structure;
    }
}
