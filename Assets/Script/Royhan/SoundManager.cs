using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("------- Audio Source -------")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip -------")]
    public AudioClip BgmFight;
    public AudioClip BGMMainMenu;
    public AudioClip[] SFXClips; // Array of sound effects audio clips
    private StartRound startRound;
    private bool fightPlayed = true;

    private void Start()
    {
        MusicSource.clip = BgmFight;
        MusicSource.loop = true;
        MusicSource.Play();

    }

    private void Update() 
    {

    }

    public void SwitchBGMMainMenu()
    {
        Debug.Log("Switch");
        MusicSource.Stop();
        MusicSource.clip = BGMMainMenu;
        fightPlayed = false;

        MusicSource.Play();
    }
    public void SwitchBGMFight()
    {
        MusicSource.Stop();
        MusicSource.clip = BgmFight;
        fightPlayed = true;
        MusicSource.Play();
    }

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
