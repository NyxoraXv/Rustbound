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

        Instantiate(turretSlot1, turret1.position, turret1.rotation);
    }
    public void turretSlotBar2()
    {

        Instantiate(turretSlot2, turret2.position, turret2.rotation);
    }
    public void turretSlotBar3()
    {

        Instantiate(turretSlot3, turret3.position, turret3.rotation);
    }
    public void turretSlotBar4()
    {

        Instantiate(turretSlot4, turret4.position, turret4.rotation);
    }
}
