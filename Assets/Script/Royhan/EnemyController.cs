using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : VariableComponent
{
    [Header("Add Currency")]
    public int currencyAdd = 1;

    public bool isBoss = false;
    public float regenerationHealthPerSecond = 10f;

    [System.Flags]
    public enum ResistanceType
    {
        None = 0,
        Plasma = 1 << 0,  // 1
        Bullet = 1 << 1,  // 2
        Explosion = 1 << 2 // 4
    }

    public enum TargetType
    {
        None,          // Target both player and turret
    }

    public Image healthBar;
    public TargetType currentTargetType = TargetType.None; // Set default target type
    public float damageDealt = 10f;
    public ResistanceType resistances = ResistanceType.None; // Set multiple resistances in the inspector
    [Range(0, 1)] public float resistanceMultiplier = 0.5f; // Adjustable resistance percentage (0.5 means 50% damage reduction)
    public float attactRange = 5f;
    public LayerMask targetMask;
    public bool setState = false;
    public int state = 0;
    // private VariableComponent variableComponent;
    private GameObject targetedEntity;
    private NavMeshAgent navMeshAgent;
    private Round round; // Reference to the Round class
    private Animator animator;
    private int attackParam = Animator.StringToHash("Attack");
    private int caseParam = Animator.StringToHash("Casing");
    private int dieParam = Animator.StringToHash("Die");
    private int stateParam = Animator.StringToHash("Anim State");
    private float targetUpdateInterval = 1f; // Update target every second
    private float nextTargetUpdateTime = 0f;
    private bool detectPlayer = false;
    private CurrencyManager currencyManager;


    private void Start()
    {
        if (!setState)
        {
            state = UnityEngine.Random.Range(0, 5);
        }

        _currentHealth = maxHealth;
        
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent nm))
        {
            navMeshAgent = nm;
        }

        // Get the Round component in the scene (make sure there's only one Round component or adjust accordingly)
        round = FindObjectOfType<Round>();
        if (round == null)
        {
            Debug.LogError("Round component not found in the scene.");
        }

        SetTarget();
        
        // Start health regeneration coroutine if the enemy is a boss
        if (isBoss)
        {
            StartCoroutine(RegenerateHealth());
        }

        if (TryGetComponent<Animator>(out Animator anim))
        {
            animator = anim;
            animator.SetInteger(stateParam,state);
        }

        currencyManager = FindAnyObjectByType<CurrencyManager>();
    }

    private void Update()
    {
        if (Time.time >= nextTargetUpdateTime)
        {
            SetTarget(); // Update the target
            nextTargetUpdateTime = Time.time + targetUpdateInterval; // Schedule next update
        }
        if (targetedEntity != null)
        {
            try{

                navMeshAgent.SetDestination(targetedEntity.transform.position); // Move towards the target
            }
            catch(MissingReferenceException)
            {
                
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attactRange, targetMask);
            detectPlayer = false;
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject == targetedEntity)
                {
                    if (targetedEntity.TryGetComponent<VariableComponent>(out VariableComponent vc)) // Bisa gunakan tag atau cek komponen spesifik
                    {
                        detectPlayer = true;
                        animator.SetBool(attackParam, true);
                        // vc.TakeDamage(damageDealt);
                    }
            
                }
            }


            if (navMeshAgent.velocity != Vector3.zero)
            {
                animator.SetBool(attackParam, false);
                animator.SetBool(caseParam, true);
            }
            else
            {
                animator.SetBool(caseParam, false);
            }

            if (!detectPlayer)
            {
                animator.SetBool(attackParam, false);
            }
        }

        if (_currentHealth <= 0)   
        {
            Die();
        }
        UpdateHealth();
    }
    private void UpdateHealth()
    {
        float healthPercentage =_currentHealth / maxHealth;; 
        if (healthPercentage<1)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.fillAmount = healthPercentage;
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
    }
    public void Del () 
    {
        Destroy(gameObject, 1.5f);
        currencyManager.AddCurrency(currencyAdd);
        if (round != null)
        {
            round.DecreaseZombieCount(gameObject); // Call the method to decrease total zombie count
        }
    }
    protected override void Die ()
    {
        navMeshAgent.speed = 0;
        animator.SetTrigger(dieParam);
        Destroy(GetComponent<Collider>());
        // Destroy(navMeshAgent);
        Destroy(GetComponent<Rigidbody>());
    }

    private GameObject FindNearestEntityWithTag(string tag)
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag(tag);
        GameObject nearestEntity = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject entity in entities)
        {
            float distance = Vector3.Distance(transform.position, entity.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEntity = entity;
            }
        }

        return nearestEntity; // Returns the nearest entity or null if none found
    }

    public void Attack()
    {
        if (targetedEntity != null)
        {
            if (targetedEntity.TryGetComponent<TurretCone>(out TurretCone turretCone))
            {
                turretCone.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretAdvance>(out TurretAdvance turretAdvance))
            {
                turretAdvance.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretBasic>(out TurretBasic turretBasic))
            {
                turretBasic.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretDoubleType>(out TurretDoubleType turretDoubleType))
            {
                turretDoubleType.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretLaser>(out TurretLaser turretLaser))
            {
                turretLaser.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretLaserAdvance>(out TurretLaserAdvance turretLaserAdvance))
            {
                turretLaserAdvance.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretMinigun>(out TurretMinigun turretMinigun))
            {
                turretMinigun.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretOverpower>(out TurretOverpower turretOverpower))
            {
                turretOverpower.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretOverpowerDouble>(out TurretOverpowerDouble turretOverpowerDouble))
            {
                turretOverpowerDouble.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretOverpowerQuad>(out TurretOverpowerQuad turretOverpowerQuad))
            {
                turretOverpowerQuad.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretOverpowerTriple>(out TurretOverpowerTriple turretOverpowerTriple))
            {
                turretOverpowerTriple.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretRocket>(out TurretRocket turretRocket))
            {
                turretRocket.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<TurretRocketAdvance>(out TurretRocketAdvance turretRocketAdvance))
            {
                turretRocketAdvance.TakeDamage(damageDealt);
            }
            else if (targetedEntity.TryGetComponent<Building>(out Building building))
            {
                building.TakeDamage(damageDealt);
            }
            else
            {
                VariableComponent targetHealth = targetedEntity.GetComponent<VariableComponent>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damageDealt); // Apply damage
                }
            }
        }
    }

    // Method to take damage, checking for resistance
    public void TakeDamage(float damage, ProjectileController.BulletType bulletType)
    {
        // Check if the enemy has resistance to the bullet type and apply damage reduction if resistant
        if (IsResistantTo(bulletType))
        {
            damage *= (1 - resistanceMultiplier); // Apply the resistance multiplier
        }

        TakeDamage(damage);
        // Optional: Check if the enemy is dead and handle accordingly
        if (!IsAlive())
        {
            // Decrease total zombies in the Round
            if (round != null)
            {
                round.DecreaseZombieCount(gameObject); // Call the method to decrease total zombie count
            }
        }

    }

    // Method to set the targeted turret
    private void SetTarget()
    {
        switch (currentTargetType)
        {
            case TargetType.None:
                // Find the nearest player, turret, and building
                GameObject nearestPlayer = FindNearestEntityWithTag("Player");
                GameObject nearestTurret = FindNearestEntityWithTag("Turret");
                GameObject nearestBuilding = FindNearestEntityWithTag("Building");

                // Choose the closest entity between player, turret, and building
                if (nearestPlayer != null || nearestTurret != null || nearestBuilding != null)
                {
                    // Initialize minimum distance and closest entity
                    float minDistance = Mathf.Infinity;
                    GameObject closestEntity = null;

                    // Check the distance to the player
                    if (nearestPlayer != null)
                    {
                        float distanceToPlayer = Vector3.Distance(transform.position, nearestPlayer.transform.position);
                        if (distanceToPlayer < minDistance)
                        {
                            minDistance = distanceToPlayer;
                            closestEntity = nearestPlayer;
                        }
                    }

                    // Check the distance to the turret
                    if (nearestTurret != null)
                    {
                        float distanceToTurret = Vector3.Distance(transform.position, nearestTurret.transform.position);
                        if (distanceToTurret < minDistance)
                        {
                            minDistance = distanceToTurret;
                            closestEntity = nearestTurret;
                        }
                    }

                    // Check the distance to the building
                    if (nearestBuilding != null)
                    {
                        float distanceToBuilding = Vector3.Distance(transform.position, nearestBuilding.transform.position);
                        if (distanceToBuilding < minDistance)
                        {
                            minDistance = distanceToBuilding;
                            closestEntity = nearestBuilding;
                        }
                    }

                    // Set the closest entity as the target
                    targetedEntity = closestEntity;
                }
                else
                {
                    // If none of the entities exist, keep the target as null
                    targetedEntity = null;
                }
                break;
        }
}

    // Check if the bullet type matches any of the enemy's resistance types
    private bool IsResistantTo(ProjectileController.BulletType bulletType)
    {
        ResistanceType bulletResistance = ConvertBulletTypeToResistanceType(bulletType);
        return (resistances & bulletResistance) != 0; // Check if any resistance flag matches
    }

    // Convert BulletType to ResistanceType for comparison
    private ResistanceType ConvertBulletTypeToResistanceType(ProjectileController.BulletType bulletType)
    {
        switch (bulletType)
        {
            case ProjectileController.BulletType.Plasma:
                return ResistanceType.Plasma;
            case ProjectileController.BulletType.Bullet:
                return ResistanceType.Bullet;
            case ProjectileController.BulletType.Explosion:
                return ResistanceType.Explosion;
            default:
                return ResistanceType.None;
        }
    }

    // Optional: Method to check if the enemy is alive
    public bool IsAlive()
    {
        return GetCurrentHealth() > 0;
    }

    private System.Collections.IEnumerator RegenerateHealth()
    {
        while (true) // Keep this loop running indefinitely
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second

            // Check if the boss is still alive
            if (!IsAlive())
            {
                yield break; // Stop the coroutine if the boss is dead
            }

            // Heal the boss if current health is less than max health
            if (GetCurrentHealth() < GetMaxHealth())
            {
                Heal(regenerationHealthPerSecond); // Heal by the specified amount
            }
        }
    }
}
