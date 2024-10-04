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
    
    public void turretSlotBar1()
    {
        if (turret1.childCount == 0) 
        {
            
            GameObject turret = Instantiate(turretSlot1, turret1.position, turret1.rotation);
            turret.transform.SetParent(turret1); 
        }
        else
        {
            Debug.Log("Turret in Slot 1 already exists. Cancelling instantiation.");
        }
    }
    
    public void turretSlotBar2()
    {
        if (turret2.childCount == 0) 
        {
            GameObject turret = Instantiate(turretSlot2, turret2.position, turret2.rotation);
            turret.transform.SetParent(turret2); 
        }
        else
        {
            Debug.Log("Turret in Slot 2 already exists. Cancelling instantiation.");
        }
    }
    
    public void turretSlotBar3()
    {
        if (turret3.childCount == 0) 
        {
            GameObject turret = Instantiate(turretSlot3, turret3.position, turret3.rotation);
            turret.transform.SetParent(turret3); 
        }
        else
        {
            Debug.Log("Turret in Slot 3 already exists. Cancelling instantiation.");
        }
    }
    
    public void turretSlotBar4()
    {
        if (turret4.childCount == 0) 
        {
            GameObject turret = Instantiate(turretSlot4, turret4.position, turret4.rotation);
            turret.transform.SetParent(turret4); 
        }
        else
        {
            Debug.Log("Turret in Slot 4 already exists. Cancelling instantiation.");
        }
    }
}
