using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiTest : MonoBehaviour
{
    public GameObject turretSlot1;
    public Transform turret1;
    public GameObject turretSlot2;
    public Transform turret2;
    public GameObject turretSlot3;
    public Transform turret3;
    public GameObject turretSlot4;
    public Transform turret4;

    // Method to instantiate turret in slot 1
    public void turretSlotBar1()
    {
        if (turret1.childCount == 0) // Check if turret1 has no children
        {
            // Instantiate turret if no turret is already in the slot
            GameObject turret = Instantiate(turretSlot1, turret1.position, turret1.rotation);
            turret.transform.SetParent(turret1); // Set the instantiated turret as a child of turret1
        }
        else
        {
            Debug.Log("Turret in Slot 1 already exists. Cancelling instantiation.");
        }
    }

    // Method to instantiate turret in slot 2
    public void turretSlotBar2()
    {
        if (turret2.childCount == 0) // Check if turret2 has no children
        {
            GameObject turret = Instantiate(turretSlot2, turret2.position, turret2.rotation);
            turret.transform.SetParent(turret2); // Set the instantiated turret as a child of turret2
        }
        else
        {
            Debug.Log("Turret in Slot 2 already exists. Cancelling instantiation.");
        }
    }

    // Method to instantiate turret in slot 3
    public void turretSlotBar3()
    {
        if (turret3.childCount == 0) // Check if turret3 has no children
        {
            GameObject turret = Instantiate(turretSlot3, turret3.position, turret3.rotation);
            turret.transform.SetParent(turret3); // Set the instantiated turret as a child of turret3
        }
        else
        {
            Debug.Log("Turret in Slot 3 already exists. Cancelling instantiation.");
        }
    }

    // Method to instantiate turret in slot 4
    public void turretSlotBar4()
    {
        if (turret4.childCount == 0) // Check if turret4 has no children
        {
            GameObject turret = Instantiate(turretSlot4, turret4.position, turret4.rotation);
            turret.transform.SetParent(turret4); // Set the instantiated turret as a child of turret4
        }
        else
        {
            Debug.Log("Turret in Slot 4 already exists. Cancelling instantiation.");
        }
    }
}
