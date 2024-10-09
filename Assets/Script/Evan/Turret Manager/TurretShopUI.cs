using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShopUI : MonoBehaviour
{
    [SerializeField] private GameObject turretContainerPrefab; // The prefab with WeaponContainerInitiator
    [SerializeField] private Transform turretContainerParent;  // The parent for instantiated prefabs
    [SerializeField] private Vector2 posAddition;
    [SerializeField] private float fadeDuration = 0.5f; // Duration for each fade-in
    private TurretDatabase turretData; // The database of weapons

    private void OnDisable()
    {
        DestroyAllChildren(turretContainerParent);
    }

    private void OnEnable()
    {
        turretData = TurretManager.Instance.turretDatabase;
        PopulateConstructionWindow();
    }

    private void PopulateConstructionWindow()
    {
        List<TurretData> allTurrets = turretData.GetAllTurret();
        StartCoroutine(FadeInTurrets(allTurrets));
    }

    private IEnumerator FadeInTurrets(List<TurretData> allTurrets)
    {
        int i = 0;
        foreach (TurretData turret in allTurrets)
        {
            // Instantiate the turret prefab and set its parent
            GameObject newTurretContainer = Instantiate(turretContainerPrefab, turretContainerParent);
            newTurretContainer.GetComponent<RectTransform>().anchoredPosition = (Vector2.zero + (posAddition * i));
            i += 1;

            // Get the TurretContainerInitiator component and initialize it
            TurretContainerInitiator initiator = newTurretContainer.GetComponent<TurretContainerInitiator>();
            initiator.Initialize(turret); // Pass the current turret data to the initializer

            // Get the CanvasGroup component and start the fade-in effect
            CanvasGroup canvasGroup = newTurretContainer.GetComponent<CanvasGroup>();
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
