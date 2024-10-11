using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [Header("------- Audio Source -------")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip -------")]
    public AudioClip BgmFight;
    public AudioClip BGMMainMenu;
    public AudioClip[] SFXClips; // Array of sound effects audio clips

    private bool fightPlayed = true;
    public float fadeDuration = 1.0f; // Durasi fade-out dan fade-in

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        MusicSource.clip = BgmFight;
        MusicSource.loop = true;
        MusicSource.Play();
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

    // Coroutine to fade out, switch BGM, and fade in
    private IEnumerator FadeOutAndSwitch(AudioSource audioSource, AudioClip newClip)
    {
        // Fade out
        float startVolume = audioSource.volume;

        // Gradually decrease the volume
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop(); // Stop the audio before switching clip
        audioSource.clip = newClip;
        audioSource.Play(); // Play the new BGM

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume; // Ensure the volume is fully restored
    }

    // Play sound effects (SFX)
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < SFXClips.Length)
        {
            SFXSource.PlayOneShot(SFXClips[index]);
        }
        else
        {
            Debug.LogWarning("Invalid SFX index!");
        }
    }
}
