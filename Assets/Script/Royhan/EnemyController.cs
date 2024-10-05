using UnityEngine;

public class EnemyController : MonoBehaviour
{
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
    private GameObject targetedEntity;

    public float damageDealt = 10f;
    public ResistanceType resistances = ResistanceType.None; // Set multiple resistances in the inspector
    private bool isAttacking = false;
    [Range(0, 1)] public float resistanceMultiplier = 0.5f; // Adjustable resistance percentage (0.5 means 50% damage reduction)

    private VariableComponent variableComponent;
    private Round round; // Reference to the Round class

    private float targetUpdateInterval = 1f; // Update target every second
    private float nextTargetUpdateTime = 0f;

    private void Start()
    {
        // Get the VariableComponent attached to this GameObject
        variableComponent = GetComponent<VariableComponent>();
        if (variableComponent == null)
        {
            Debug.LogError("VariableComponent not found on this GameObject.");
        }

        // Get the Round component in the scene (make sure there's only one Round component or adjust accordingly)
        round = FindObjectOfType<Round>();
        if (round == null)
        {
            Debug.LogError("Round component not found in the scene.");
        }

        SetTarget();
        // Attack();
    }
    private void Update()
    {
        if (Time.time >= nextTargetUpdateTime)
        {
            SetTarget(); // Update the target
            nextTargetUpdateTime = Time.time + targetUpdateInterval; // Schedule next update
        }
        
        // Call the Attack method based on certain conditions
        // Attack();
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
        if (variableComponent != null)
        {
            // Check if the enemy has resistance to the bullet type and apply damage reduction if resistant
            if (IsResistantTo(bulletType))
            {
                damage *= (1 - resistanceMultiplier); // Apply the resistance multiplier
            }

            variableComponent.TakeDamage(damage);

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
        if (targetedEntity != null)
        {
            Debug.Log($"Target set to: {targetedEntity.name}");
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
        return variableComponent != null && variableComponent.GetCurrentHealth() > 0;
    }
}
