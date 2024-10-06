using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretDatabase", menuName = "Database/TurretDatabase")]
public class TurretDatabase : ScriptableObject
{
    public List<TurretData> turrets;

    // Method to get turret by its ID
    public TurretData GetTurretByID(int turretID)
    {
        return turrets.Find(turret => turret.turretID == turretID);
    }
}
