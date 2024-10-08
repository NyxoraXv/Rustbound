using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretDatabase", menuName = "Database/TurretDatabase")]
public class TurretDatabase : ScriptableObject
{
    public List<TurretData> turrets;

    public TurretData GetTurretByID(int ID)
    {
        return turrets.Find(turret => turret.ID == ID);
    }

    public List<TurretData> GetAllTurret()
    {
        return turrets;
    }
}
