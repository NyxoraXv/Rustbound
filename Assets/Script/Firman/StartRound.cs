using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Import DoTween

public class StartRound : MonoBehaviour
{
    [Header("Round Begin")]
    public GameObject roundBegin;

    public GameObject uiToFade;
    private SoundManager soundManager;
    private Round round;
    private float targetUpdateInterval = 1f; // Update target every second
    private float nextTargetUpdateTime = 0f;
    private CanvasGroup canvasGroup;

    void Start()
    {
        round = FindObjectOfType<Round>();
        soundManager = FindAnyObjectByType<SoundManager>();

        // Ensure the GameObject has a CanvasGroup for fading
        canvasGroup = uiToFade.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = uiToFade.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (Time.time >= nextTargetUpdateTime)
        {
            if (round.AreAllZombiesDead())
            {
                FadeInUI(); // Example: Fade out UI when all zombies are dead
            }
            nextTargetUpdateTime = Time.time + targetUpdateInterval; // Schedule next update
        }
    }
    public void TriggerRoundBegin()
    {
        CanvasGroup canvasGroup = roundBegin.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = roundBegin.AddComponent<CanvasGroup>(); // Add CanvasGroup if not present
        }

        roundBegin.SetActive(true);  // Step 1: Set active

        canvasGroup.alpha = 0f;
        
            canvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                // Step 3: Optional delay after fading in
                DOVirtual.DelayedCall(2f, () =>
                {
                    // Step 4: Fade out the roundFinish
                    canvasGroup.DOFade(0f, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        // Step 5: Set inactive after fade out
                        roundBegin.SetActive(false);
                    });
                });
            });

    }

    public void StartNewRound()
    {
        Debug.Log("Start New Round");
        TriggerRoundBegin();
        soundManager.SwitchBGMFight();
        StartCoroutine(round.StartSpawningZombies());
        StartCoroutine(round.SpawnSpecialZombiesWithDelay());

        FadeOutUI(); // Example: Fade in UI when a new round starts
    }

    private void FadeInUI()
    {
        canvasGroup.DOFade(1f, 3f); // Fade in over 1 second
    }

    private void FadeOutUI()
    {
        canvasGroup.DOFade(0f, 3f); // Fade out over 1 second
    }
}
