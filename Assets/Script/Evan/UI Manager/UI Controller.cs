using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private int currentState;
    public GameObject HUD, WeaponMarket, TurretMarket, Inventory, PlacementUI;

    private void Start()
    {
        HUDSetactive(false);

        DOVirtual.DelayedCall(5f, () => { HUDSetactive(true); });
    }

    public void HUDSetactive(bool setActive) 
    {
        if (!setActive)
        {
            HUD.transform.DOScale(2f, 1f).OnStart(() =>
            {
                HUD.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
            }).OnComplete(() => { HUD.SetActive(false); });
        }
        else
        {
            HUD.SetActive(true);
            HUD.transform.DOScale(1f, 1f).OnStart(() =>
            {
                HUD.GetComponent<CanvasGroup>().DOFade(1f, 2f);
            });
        }
    }

    public void WeaponMarketSetactive(bool setActive)
    {
        if (!setActive)
        {

        }
        else
        {

        }
    }

    public void setUIState(int stateID)
    {
        currentState = stateID;

        switch (currentState)
        {
            case 0:
                HUD.SetActive(true);
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 1:
                HUD.SetActive(false);
                WeaponMarket.SetActive(true);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 2:
                HUD.SetActive(false);
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(true);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 3:
                HUD.SetActive(false);
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(true);
                PlacementUI.SetActive(false);
                break;
            case 4:
                HUD.SetActive(false);
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(true);
                break;
            default:
                HUD.SetActive(true);
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
        }
    }
}