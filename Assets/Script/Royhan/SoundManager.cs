using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("------- Audio Source -------")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource[] SFXSources; // Array of AudioSources for SFX

    [Header("------- Audio Clip -------")]
    public AudioClip BgmFight;
    public AudioClip BGMMainMenu;
    public AudioClip BGMMarket;
    public AudioClip[] SFXClips; // Array of sound effects audio clips

    private bool fightPlayed = true;
    public float fadeDuration = 1.0f; // Durasi fade-out dan fade-in
    private AudioClip lastAudio;
    private int currentSFXIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        MusicSource.clip = BgmFight;
        MusicSource.loop = true;
        MusicSource.Play();
        MusicSource.priority = 128;
        foreach (var item in SFXSources)
        {
            item.priority = 256;
        }
    }

    // Switch BGM to Main Menu with Fade Out/In
    public void SwitchBGMMainMenu()
    {
        if (!fightPlayed) return; // Cegah jika sudah di MainMenu
        StartCoroutine(FadeOutAndSwitch(MusicSource, BGMMainMenu));
        fightPlayed = false;
    }

    // Switch BGM to Fight with Fade Out/In
    public void SwitchBGMFight()
    {
        if (fightPlayed) return; // Cegah jika sudah di Fight mode
        StartCoroutine(FadeOutAndSwitch(MusicSource, BgmFight));
        fightPlayed = true;
    }

    public void SwitchMarket()
    {
        if (MusicSource.clip != BGMMarket)
        {
            lastAudio = MusicSource.clip;
            StartCoroutine(FadeOutAndSwitch(MusicSource, BGMMarket));
        }
    }

    public void ReturnSwitchMarket()
    {
        if (MusicSource.clip == BGMMarket)
            StartCoroutine(FadeOutAndSwitch(MusicSource, lastAudio));
    }

    // Coroutine to fade out, switch BGM, and fade in
    private IEnumerator FadeOutAndSwitch(AudioSource audioSource, AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop(); 
        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
    }

    // Play sound effects (SFX) using round-robin through available sources
    public void PlaySFX(int index, float volume = 1f)
    {
        if (index >= 0 && index < SFXClips.Length)
        {
            AudioSource currentSFXSource = SFXSources[currentSFXIndex];
            currentSFXSource.volume = volume;
            currentSFXSource.PlayOneShot(SFXClips[index]);

            currentSFXIndex = (currentSFXIndex + 1) % SFXSources.Length; // Round-robin increment
        }
        else
        {
            Debug.LogWarning("Invalid SFX index!");
        }
    }
}
