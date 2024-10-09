using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShopUI : MonoBehaviour
{
    [SerializeField] private GameObject weaponContainerPrefab; // The prefab with WeaponContainerInitiator
    [SerializeField] private Transform weaponContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 posAddition;
    [SerializeField] private float fadeDuration = 0.5f; // Duration for each fade-in
    private WeaponDatabase weaponDatabase;    // The database of weapons

    private void OnDisable()
    {
        DestroyAllChildren(weaponContainerParent);
    }


    private void OnEnable()
    {
        weaponDatabase = WeaponManager.Instance.weaponDatabase;
        PopulateWeaponInventory();
    }

    private void PopulateWeaponInventory()
    {
        List<WeaponData> allWeapons = weaponDatabase.GetAllWeapons();
        StartCoroutine(FadeInWeapons(allWeapons));
    }

    private IEnumerator FadeInWeapons(List<WeaponData> allWeapons)
    {
        int i = 0;
        foreach (WeaponData weapon in allWeapons)
        {
            // Instanti ate the weapon prefab and set its parent
            GameObject newWeaponContainer = Instantiate(weaponContainerPrefab, weaponContainerParent);
            newWeaponContainer.GetComponent<RectTransform>().anchoredPosition = (Vector2.zero + (posAddition * i));
            i += 1;

            // Get the WeaponContainerInitiator component and initialize it
            WeaponContainerInitiator initiator = newWeaponContainer.GetComponent<WeaponContainerInitiator>();
            initiator.Initialize(weapon); // Pass the current weapon data to the initializer

            // Get the CanvasGroup component and start the fade-in effect
            CanvasGroup canvasGroup = newWeaponContainer.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0; // Start with an invisible container
            yield return StartCoroutine(FadeIn(canvasGroup, fadeDuration));
        }
    }

    private void DestroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1; // Ensure it's fully visible at the end
    }
}