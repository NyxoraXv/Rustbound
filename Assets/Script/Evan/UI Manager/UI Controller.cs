using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIController : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;
    private int currentState;
    public GameObject HUD, WeaponMarket, TurretMarket, Inventory, PlacementUI;

    private void Start()
    {
        HUDSetactive(false);

        DOVirtual.DelayedCall(5f, () => { HUDSetactive(true); });
        globalVolume.profile.TryGet<Vignette>(out vignette);
        globalVolume.profile.TryGet<DepthOfField>(out depthOfField);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.O)) {
        setUIState(currentState+1);
        }
    }

    public void triggerGlobalVolume(bool setActive)
    {
        if (setActive)
        {
            StartCoroutine(TweenGlobalVolume(0.5f, 20f, 1f)); // Tween to 0.5 and 20 over 1 second
        }
        else
        {
            StartCoroutine(TweenGlobalVolume(0f, 15f, 1f)); // Tween to 0 and 15 over 1 second
        }
    }

    private IEnumerator TweenGlobalVolume(float targetVignetteIntensity, float targetFocusDistance, float duration)
    {
        float startVignetteIntensity = vignette.intensity.value;
        float startFocusDistance = depthOfField.focusDistance.value;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            vignette.intensity.value = Mathf.Lerp(startVignetteIntensity, targetVignetteIntensity, normalizedTime);
            depthOfField.focusDistance.value = Mathf.Lerp(startFocusDistance, targetFocusDistance, normalizedTime);
            yield return null;
        }

        // Ensure we set to final value
        vignette.intensity.value = targetVignetteIntensity;
        depthOfField.focusDistance.value = targetFocusDistance;
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
            // Deactivate WeaponMarket
        }
        else
        {
            // Activate WeaponMarket
        }
    }

    public void setUIState(int stateID)
    {
        currentState = stateID;

        // Deactivate global volume if outside states 1, 2, or 3
        if (stateID < 1 || stateID > 3)
        {
            triggerGlobalVolume(false);
        }

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
                triggerGlobalVolume(true);  // Trigger for state 1
                HUD.SetActive(false);
                WeaponMarket.SetActive(true);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 2:
                triggerGlobalVolume(true);  // Trigger for state 2
                HUD.SetActive(false);
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(true);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 3:
                triggerGlobalVolume(true);  // Trigger for state 3
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
