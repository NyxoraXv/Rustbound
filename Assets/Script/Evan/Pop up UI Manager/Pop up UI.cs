using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    [SerializeField] private int UIID;
    [SerializeField] private KeyCode popUpKey;
    [SerializeField] private bool collide;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Colliding");
            collide = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collide = false;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(popUpKey))
        {
            Debug.Log("Key pressed: " + popUpKey.ToString());
            Debug.Log("Collide: " + collide);
        }

        if (Input.GetKeyDown(popUpKey) && collide)
        {
            Debug.Log("Attempting to change UI");
            changeUI();
        }
    }


    private void changeUI()
    {
        UIController.instance.setUIState(UIID);
    }

}