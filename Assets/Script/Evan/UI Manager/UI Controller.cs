using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    
    [SerializeField] private Volume globalVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;
    private int currentState;
    public GameObject HUD, WeaponMarket, TurretMarket, Inventory, PlacementUI;

    private bool isHUDAnimating = false; // Track if the HUD is currently animating

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        globalVolume.profile.TryGet<Vignette>(out vignette);
        globalVolume.profile.TryGet<DepthOfField>(out depthOfField);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            setUIState(4);
        }
        if(Input.GetKeyDown(KeyCode.I)) {
            setUIState(3);
        }
        if(Input.GetKeyDown(KeyCode.Escape) && currentState!=0)
        {
            setUIState(0);
        }
    }

    public void triggerGlobalVolume(bool setActive)
    {
        if (setActive)
        {
            StartCoroutine(TweenGlobalVolume(0.5f, 55f, 1f)); // Tween to 0.5 and 20 over 1 second
        }
        else
        {
            StartCoroutine(TweenGlobalVolume(0f, 15f, 1f)); // Tween to 0 and 15 over 1 second
        }
    }

    private IEnumerator TweenGlobalVolume(float targetVignetteIntensity, float targetFocusDistance, float duration)
    {
        float startVignetteIntensity = vignette.intensity.value;
        float startFocusDistance = depthOfField.focalLength.value;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            vignette.intensity.value = Mathf.Lerp(startVignetteIntensity, targetVignetteIntensity, normalizedTime);
            depthOfField.focalLength.value = Mathf.Lerp(startFocusDistance, targetFocusDistance, normalizedTime);
            yield return null;
        }

        // Ensure we set to final value
        vignette.intensity.value = targetVignetteIntensity;
        depthOfField.focalLength.value = targetFocusDistance;
    }

    public IEnumerator HUDSetactive(bool setActive)
    {
        if (isHUDAnimating) yield break; // Exit if the HUD is already animating

        isHUDAnimating = true; // Mark HUD as animating

        if (!setActive)
        {
            HUD.transform.DOScale(2f, 1f).OnStart(() =>
            {
                HUD.GetComponent<CanvasGroup>().DOFade(0f, 1f);
            }).OnComplete(() =>
            {
                HUD.SetActive(false);
                isHUDAnimating = false; // Reset the animating flag
            });
            yield return new WaitForSeconds(1f); // Wait for the scaling animation to finish
        }
        else
        {
            HUD.SetActive(true);
            HUD.transform.DOScale(1f, 1f).OnStart(() =>
            {
                HUD.GetComponent<CanvasGroup>().DOFade(1f, 2f);
            }).OnComplete(() =>
            {
                isHUDAnimating = false; // Reset the animating flag
            });
            yield return new WaitForSeconds(2f); // Wait for the fade-in animation to finish
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
        // If HUD is animating, wait for it to finish
        if (isHUDAnimating)
        {
            return; // Prevent state change if HUD is animating
        }

        currentState = stateID;

        // Deactivate global volume if outside states 1, 2, or 3
        if (stateID < 1 || stateID > 3)
        {
            triggerGlobalVolume(false);
        }

        // Start the HUD animation based on the current state
        switch (currentState)
        {
            case 0:
                StartCoroutine(HUDSetactive(true));
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 1:
                StartCoroutine(HUDSetactive(false)); // Hide HUD and wait for the animation
                triggerGlobalVolume(true); // Trigger for state 1
                WeaponMarket.SetActive(true);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 2:
                StartCoroutine(HUDSetactive(false)); // Hide HUD and wait for the animation
                triggerGlobalVolume(true); // Trigger for state 2
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(true);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
            case 3:
                StartCoroutine(HUDSetactive(false)); // Hide HUD and wait for the animation
                triggerGlobalVolume(true); // Trigger for state 3
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(true);
                PlacementUI.SetActive(false);
                break;
            case 4:
                StartCoroutine(HUDSetactive(false)); // Hide HUD and wait for the animation
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(true);
                break;
            default:
                StartCoroutine(HUDSetactive(false)); // Hide HUD and wait for the animation
                WeaponMarket.SetActive(false);
                TurretMarket.SetActive(false);
                Inventory.SetActive(false);
                PlacementUI.SetActive(false);
                break;
        }
    }
}
