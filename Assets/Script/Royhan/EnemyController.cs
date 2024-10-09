using System;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : VariableComponent
{
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
        NearestTurret, // Target only the nearest turret
        NearestPlayer   // Target only the nearest player
    }

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

    private void Start()
    {
        // Get the VariableComponent attached to this GameObject
        // variableComponent = GetComponent<VariableComponent>();
        // if (variableComponent == null)
        // {
        //     Debug.LogError("VariableComponent not found on this GameObject.");
        // }

        if (!setState)
        {
            state = UnityEngine.Random.Range(0, 5);
        }

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
            Debug.Log("Boss detected. Starting health regeneration.");
            StartCoroutine(RegenerateHealth());
        }

        if (TryGetComponent<Animator>(out Animator anim))
        {
            animator = anim;
            animator.SetInteger(stateParam,state);
        }
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
                    Debug.Log("collide");
                    if (targetedEntity.TryGetComponent<VariableComponent>(out VariableComponent vc)) // Bisa gunakan tag atau cek komponen spesifik
                    {
                        detectPlayer = true;
                        animator.SetBool(attackParam, true);
                        // vc.TakeDamage(damageDealt);
                        Debug.Log($"{targetedEntity.name} attacked! Damage dealt: {damageDealt}");
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

        // if (_currentHealth <= 0)   Die();
    }
    public void Del () => Destroy(gameObject, 0.2f);
    protected override void Die ()
    {
        navMeshAgent.speed = 0;
        animator.SetTrigger(dieParam);
        GetComponent<Collider>().enabled = false;
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
            VariableComponent targetHealth = targetedEntity.GetComponent<VariableComponent>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageDealt); // Apply damage
                Debug.Log($"{targetedEntity.name} attacked! Damage dealt: {damageDealt}");
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
            Debug.Log("Enemy is dead!");
        }
    }

    // Method to set the targeted turret
    private void SetTarget()
    {
        switch (currentTargetType)
        {
            case TargetType.None:
                // Find the nearest player and turret
                GameObject nearestPlayer = FindNearestEntityWithTag("Player");
                GameObject nearestTurret = FindNearestEntityWithTag("Turret");

                // Choose the closest entity between player and turret
                if (nearestPlayer != null && nearestTurret != null)
                {
                    // Compare distances and choose the closest
                    float distanceToPlayer = Vector3.Distance(transform.position, nearestPlayer.transform.position);
                    float distanceToTurret = Vector3.Distance(transform.position, nearestTurret.transform.position);

                    targetedEntity = (distanceToPlayer < distanceToTurret) ? nearestPlayer : nearestTurret;
                }
                else
                {
                    // If only one exists, target that one
                    targetedEntity = nearestPlayer ?? nearestTurret;
                }
                break;

            case TargetType.NearestTurret:
                targetedEntity = FindNearestEntityWithTag("Turret") ?? FindNearestEntityWithTag("Player");
                break;

            case TargetType.NearestPlayer:
                targetedEntity = FindNearestEntityWithTag("Player") ?? FindNearestEntityWithTag("Turret");
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
        Debug.Log("Health regeneration coroutine started.");
        while (true) // Keep this loop running indefinitely
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second

            // Check if the boss is still alive
            if (!IsAlive())
            {
                Debug.Log("Boss is dead. Stopping regeneration.");
                yield break; // Stop the coroutine if the boss is dead
            }

            // Heal the boss if current health is less than max health
            if (GetCurrentHealth() < GetMaxHealth())
            {
                Heal(regenerationHealthPerSecond); // Heal by the specified amount
                Debug.Log($"Boss healed for {regenerationHealthPerSecond} HP! Current Health: {GetCurrentHealth()}");
            }
            else
            {
                Debug.Log("Boss is at max health. No healing performed.");
            }
        }
    }
}
