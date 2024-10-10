using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRound : MonoBehaviour
{
    private Round round;
    // Start is called before the first frame update
    void Start()
    {
        round = FindObjectOfType<Round>();
    }

    public void StartRound()
    {
        Debug.Log("Start New Round");
        StartCoroutine(round.StartSpawningZombies());
        StartCoroutine(round.SpawnSpecialZombiesWithDelay());
    }
}
