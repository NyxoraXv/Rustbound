using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Round : MonoBehaviour
{
    [Header("Round Information")]
    public TextMeshProUGUI textRound; // Public variable to assign in the Inspector
    public TextMeshProUGUI textTotalZombie; // Public variable to assign in the Inspector

    [Header("Gameplay Settings")]
    public int currentRound = 1; // Example of a variable under a header

    [Header("Zombie to Spawn")]
    public int zombiesToSpawn = 5; // Maximum number of zombies to spawn

    private int spawnedZombies = 0; // Track the number of zombies spawned
    private List<GameObject> zombieList = new List<GameObject>(); // List to keep track of spawned zombies

    [Header("Bosses")]
    public GameObject Boss1; // Reference to Boss1 GameObject
    public GameObject Boss2; // Reference to Boss2 GameObject
    public GameObject Boss3; // Reference to Boss3 GameObject
    public GameObject Boss4; // Reference to Boss4 GameObject
    public GameObject Boss5; // Reference to Boss5 GameObject

    [Header("Special Zombie")]
    public GameObject[] specialZombie; // Array to hold special zombie prefabs

    [Header("Zombie Type")]
    public GameObject[] zombiePrefabs; // Array to hold different zombie prefabs

    [Header("Enemy Spawn")]
    public Transform[] spawnPoints; // Array to hold different spawn points

    private Dictionary<Transform, float> spawnPointCooldowns = new Dictionary<Transform, float>();

    void Start()
    {
        // Check if textMeshPro is assigned
        if (textRound == null)
        {
            // Try to find the TextMeshPro component if not assigned
            textRound = GetComponent<TextMeshProUGUI>();
        }

        // Update the text
        UpdateRoundText(currentRound); // For example, "ROUND 1"

        // Start spawning zombies
        StartCoroutine(StartSpawningZombies());
    }

    void Update()
    {
        // Check if all zombies are dead
        if (spawnedZombies == zombiesToSpawn && AreAllZombiesDead())
        {
            // Increment round
            currentRound++;
            UpdateRoundText(currentRound); // Update the round text
            spawnedZombies = 0; // Reset the count for the next round

            // Increment zombies to spawn by 2 for the next round
            zombiesToSpawn += 2;

            // Optionally, spawn the boss based on the current round
            SpawnBossIfNeeded();

            // Spawn special zombies with a delay
            StartCoroutine(SpawnSpecialZombiesWithDelay());

            // Start spawning zombies for the next round
            StartCoroutine(StartSpawningZombies());
        }
    }

    // Method to update the round text
    public void UpdateRoundText(int roundNumber)
    {
        if (textRound != null)
        {
            textRound.text = "ROUND " + roundNumber;
        }
        else
        {
            Debug.LogError("TextMeshPro component is missing!");
        }
    }
    

    // Coroutine to start spawning zombies
    private IEnumerator StartSpawningZombies()
    {
        // Call SpawnZombie method repeatedly until reaching the max limit
        while (spawnedZombies < zombiesToSpawn)
        {
            SpawnZombie();
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds before spawning the next zombie
        }
    }
    // Method to update the total zombie text
    public void UpdateTotalZombieText(int totalZombies)
    {
        if (textTotalZombie != null)
        {
            textTotalZombie.text = "Total Zombie " + totalZombies;
        }
        else
        {
            Debug.LogError("TextMeshPro component for total zombies is missing!");
        }
    }
    // Add this method in your Round class
    public void DecreaseZombieCount()
    {
        // Decrease the total zombies count and update the UI
        spawnedZombies--;
        UpdateTotalZombieText(spawnedZombies);
    }

    // Method to spawn a zombie
    private void SpawnZombie()
    {
        // Check if the max number of zombies has been reached
        if (spawnedZombies >= zombiesToSpawn)
            return;

        // Randomly select a zombie type
        GameObject zombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];

        // Randomly select a spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the zombie at the chosen spawn point
        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

        // Set the parent of the zombie to the spawn point
        zombie.transform.SetParent(spawnPoint);

        // Add the spawned zombie to the list
        zombieList.Add(zombie);

        // Increment the number of spawned zombies
        spawnedZombies++;

        // Update the total zombies text
        UpdateTotalZombieText(spawnedZombies);

        // Optional: Log the spawning of zombies for debugging
        Debug.Log("Spawned Zombie: " + zombiePrefab.name + " at " + spawnPoint.position);
    }
    
    // Coroutine to spawn special zombies with a delay between each
    private IEnumerator SpawnSpecialZombiesWithDelay()
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds before spawning special zombies

        // Determine the number of special zombies to spawn based on the current round
        int specialZombiesToSpawn = GetSpecialZombiesToSpawn();

        // Spawn each special zombie with a delay of 5 seconds between them
        for (int i = 0; i < specialZombiesToSpawn; i++)
        {
            SpawnSpecialZombie();
            yield return new WaitForSeconds(5f); // Wait for 5 seconds before spawning the next special zombie
        }

        // Spawn the boss after 20 seconds
        yield return new WaitForSeconds(20f);
        SpawnBossIfNeeded();
    }

    // Method to get the number of special zombies to spawn based on the current round
    private int GetSpecialZombiesToSpawn()
    {
        int specialZombiesToSpawn = 0;

        // Determine the number of special zombies to spawn based on the current round
        if (currentRound == 9)
        {
            specialZombiesToSpawn = 1;
        }
        else if (currentRound == 13)
        {
            specialZombiesToSpawn = 1;
        }
        else if (currentRound == 18)
        {
            specialZombiesToSpawn = 2;
        }
        else if (currentRound == 23)
        {
            specialZombiesToSpawn = 3;
        }
        else if (currentRound >= 25 && currentRound <= 30)
        {
            specialZombiesToSpawn = 3;
        }
        else if (currentRound >= 31 && currentRound <= 40)
        {
            specialZombiesToSpawn = 4;
        }
        else if (currentRound >= 41 && currentRound <= 50)
        {
            specialZombiesToSpawn = 5;
        }

        return specialZombiesToSpawn;
    }

    // Method to spawn a special zombie
    private void SpawnSpecialZombie()
    {
        // Randomly select a special zombie type
        GameObject specialZombiePrefab = specialZombie[Random.Range(0, specialZombie.Length)];

        // Randomly select a spawn point, ensuring to delay if the same spawn point was recently used
        Transform spawnPoint = GetSpawnPointWithDelay();

        // Instantiate the special zombie at the chosen spawn point
        Instantiate(specialZombiePrefab, spawnPoint.position, spawnPoint.rotation);

        // Optional: Log the spawning of special zombies for debugging
        Debug.Log("Spawned Special Zombie: " + specialZombiePrefab.name + " at " + spawnPoint.position);
    }

    // Method to get a spawn point with a delay if the same point was recently used
    private Transform GetSpawnPointWithDelay()
    {
        Transform spawnPoint;
        
        // Check for cooldowns and remove any that are expired
        List<Transform> expiredPoints = new List<Transform>();
        foreach (var kvp in spawnPointCooldowns)
        {
            if (Time.time >= kvp.Value)
            {
                expiredPoints.Add(kvp.Key);
            }
        }
        
        foreach (var expiredPoint in expiredPoints)
        {
            spawnPointCooldowns.Remove(expiredPoint);
        }

        // Loop until we find a spawn point that is not on cooldown
        do
        {
            // Randomly select a spawn point
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        } while (spawnPointCooldowns.ContainsKey(spawnPoint)); // Repeat until a new spawn point is found

        // Add the selected spawn point to cooldowns with a timer
        spawnPointCooldowns[spawnPoint] = Time.time + 3f; // Set cooldown for 3 seconds

        return spawnPoint;
    }

    // Method to spawn a boss if needed based on the current round
    private void SpawnBossIfNeeded()
    {
        switch (currentRound)
        {
            case 10:
                SpawnBoss(Boss1);
                break;
            case 20:
                SpawnBoss(Boss2);
                break;
            case 30:
                SpawnBoss(Boss3);
                break;
            case 40:
                SpawnBoss(Boss4);
                break;
            case 50:
                SpawnBoss(Boss5);
                break;
            default:
                break;
        }
    }

    // Method to spawn a boss
    private void SpawnBoss(GameObject boss)
    {
        if (boss != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Spawned Boss: " + boss.name + " at " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("Boss prefab is not assigned!");
        }
    }

    // Method to check if all zombies are dead
    private bool AreAllZombiesDead()
    {
        foreach (GameObject zombie in zombieList)
        {
            if (zombie != null) // Check if the zombie is still alive
            {
                return false; // If any zombie is alive, return false
            }
        }
        return true; // All zombies are dead
    }
}
