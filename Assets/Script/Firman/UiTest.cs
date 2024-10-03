using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiTest : MonoBehaviour
{
    public GameObject turretSlot1; // The prefab or object to instantiate
    public Transform turret1;      // The location where the object will be instantiated
    public GameObject turretSlot2; // The prefab or object to instantiate
    public Transform turret2;
    public GameObject turretSlot3; // The prefab or object to instantiate
    public Transform turret3;
    public GameObject turretSlot4; // The prefab or object to instantiate
    public Transform turret4;

    public void turretSlotBar1()
    {
        // Instantiate the turretSlot1 at the position and rotation of turret1
        Instantiate(turretSlot1, turret1.position, turret1.rotation);
    }
    public void turretSlotBar2()
    {
        // Instantiate the turretSlot1 at the position and rotation of turret1
        Instantiate(turretSlot2, turret2.position, turret2.rotation);
    }
    public void turretSlotBar3()
    {
        // Instantiate the turretSlot1 at the position and rotation of turret1
        Instantiate(turretSlot3, turret3.position, turret3.rotation);
    }
    public void turretSlotBar4()
    {
        // Instantiate the turretSlot1 at the position and rotation of turret1
        Instantiate(turretSlot4, turret4.position, turret4.rotation);
    }
}
