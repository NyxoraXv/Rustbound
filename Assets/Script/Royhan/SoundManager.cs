using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("------- Audio Source -------")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip -------")]
    public AudioClip Background;
    public AudioClip[] SFXClips; // Array of sound effects audio clips

    private void Start()
    {
        MusicSource.clip = Background;
        MusicSource.loop = true;
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
