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
    private List<GameObject> specialZombieList = new List<GameObject>(); // List for special zombies
    private List<GameObject> bossList = new List<GameObject>();

    [Header("Bosses")]
    public GameObject Boss1; // Reference to Boss1 GameObject
    public GameObject Boss2; // Reference to Boss2 GameObject
    public GameObject Boss3; // Reference to Boss3 GameObject
    public GameObject Boss4; // Reference to Boss4 GameObject
    public GameObject Boss5; // Reference to Boss5 GameObject

    [Header("Special Zombie Round 9")]
    public GameObject specialZombieRound9;

    [Header("Special Zombie Round 13")]
    public GameObject specialZombieRound13;

    [Header("Special Zombie Round 18")]
    public GameObject specialZombieRound18;

    [Header("Special Zombie Round 23")]
    public GameObject specialZombieRound23;

    [Header("Special Zombie Round 25+")]
    public GameObject[] specialZombie; // Array to hold special zombie prefabs

    [Header("Zombie Type")]
    public GameObject[] zombiePrefabs; // Array to hold different zombie prefabs
    public int healthIncreaseEachRound;
    private float healthMultiplier = 1;

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

            healthMultiplier += 1f;

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
    public void UpdateTotalZombieText()
    {
        // Calculate total zombies as the sum of zombies in all lists
        int totalZombies = zombieList.Count + specialZombieList.Count + bossList.Count;

        if (textTotalZombie != null)
        {
            textTotalZombie.text = "Total Zombies: " + totalZombies;
        }
        else
        {
            Debug.LogError("TextMeshPro component for total zombies is missing!");
        }
    }
    // Add this method in your Round class
    public void DecreaseZombieCount(GameObject zombie)
    {
        if (zombieList.Remove(zombie))
        {
            Debug.Log("Regular Zombie defeated.");
        }
        else if (specialZombieList.Remove(zombie))
        {
            Debug.Log("Special Zombie defeated.");
        }
        else if (bossList.Remove(zombie))
        {
            Debug.Log("Boss defeated.");
        }

        // Update the total zombies text after a zombie is defeated
        UpdateTotalZombieText();
    }

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

        // Increase the max health of this specific zombie
        IncreaseZombieMaxHealth(zombie, healthIncreaseEachRound * healthMultiplier);

        // Update the total zombies text
        UpdateTotalZombieText();

        // Optional: Log the spawning of zombies for debugging
        Debug.Log("Spawned Zombie: " + zombiePrefab.name + " at " + spawnPoint.position);
    }

    
    // Coroutine to spawn special zombies with a delay between each
    private IEnumerator SpawnSpecialZombiesWithDelay()
    {
        yield return new WaitForSeconds(1.5f); // Wait for 5 seconds before spawning special zombies

        // Determine the number of special zombies to spawn based on the current round
        int specialZombiesToSpawn = GetSpecialZombiesToSpawn();

        // Spawn each special zombie with a delay of 5 seconds between them
        for (int i = 0; i < specialZombiesToSpawn; i++)
        {
            SpawnSpecialZombie();
            yield return new WaitForSeconds(1.5f); // Wait for 5 seconds before spawning the next special zombie
        }

        // Spawn the boss after 20 seconds
        yield return new WaitForSeconds(5f);
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
       GameObject specialZombiePrefab;

        // Check if it's one of the special rounds (9, 13, 18, or 23)
        if (currentRound == 9)
        {
            specialZombiePrefab = specialZombieRound9; // Use the special zombie for these rounds
        }
        else if (currentRound == 13)
        {
            specialZombiePrefab = specialZombieRound13; // Use the special zombie for these rounds
        }
        else if (currentRound == 18)
        {
            specialZombiePrefab = specialZombieRound18; // Use the special zombie for these rounds
        }
        else if (currentRound == 23)
        {
            specialZombiePrefab = specialZombieRound23; // Use the special zombie for these rounds
        }
        else
        {
            // Randomly select a special zombie type from the array for other rounds
            specialZombiePrefab = specialZombie[Random.Range(0, specialZombie.Length)];
        }

        // Randomly select a spawn point, ensuring to delay if the same spawn point was recently used
        Transform spawnPoint = GetSpawnPointWithDelay();

        // Instantiate the special zombie at the chosen spawn point
        GameObject specialZombieInstance = Instantiate(specialZombiePrefab, spawnPoint.position, spawnPoint.rotation);

        // Make the special zombie a child of the spawn point
        specialZombieInstance.transform.SetParent(spawnPoint);
        specialZombieList.Add(specialZombieInstance);
        UpdateTotalZombieText();

        IncreaseZombieMaxHealth(specialZombieInstance, healthIncreaseEachRound * healthMultiplier);
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
            GameObject bossInstance = Instantiate(boss, spawnPoint.position, spawnPoint.rotation);
            bossInstance.transform.SetParent(spawnPoint);
            bossList.Add(bossInstance);
            
            IncreaseZombieMaxHealth(bossInstance, healthIncreaseEachRound * healthMultiplier);

            UpdateTotalZombieText();
                
            Debug.Log("Spawned Boss: " + boss.name + " at " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("Boss prefab is not assigned!");
        }
    }
    private void IncreaseZombieMaxHealth(GameObject zombie, float healthIncrease)
    {
        // Assuming the zombie has a VariableComponent to manage its health
        var variableComponent = zombie.GetComponent<VariableComponent>(); // Replace 'VariableComponent' with your actual component

        if (variableComponent != null)
        {
            variableComponent.maxHealth += healthIncrease; // Increase max health by the specified amount
        }
        else
        {
            Debug.LogError("VariableComponent is missing on the zombie!");
        }
    }

    // Method to check if all zombies are dead
    private bool AreAllZombiesDead()
    {
        // Check if all regular zombies are dead
        foreach (GameObject zombie in zombieList)
        {
            if (zombie != null)
                return false;
        }

        // Check if all special zombies are dead
        foreach (GameObject specialZombie in specialZombieList)
        {
            if (specialZombie != null)
                return false;
        }

        // Check if all bosses are dead
        foreach (GameObject boss in bossList)
        {
            if (boss != null)
                return false;
        }

        return true;
    }
}
