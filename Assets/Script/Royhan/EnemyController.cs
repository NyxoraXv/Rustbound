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

    public ResistanceType resistances = ResistanceType.None; // Set multiple resistances in the inspector
    [Range(0, 1)] public float resistanceMultiplier = 0.5f; // Adjustable resistance percentage (0.5 means 50% damage reduction)

    private VariableComponent variableComponent;
    private Round round; // Reference to the Round class

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

    public void AttackTurret(GameObject turret, float damage)
    {
        // Get the VariableComponent attached to the turret
        VariableComponent turretHealth = turret.GetComponent<VariableComponent>();
        if (turretHealth != null)
        {
            // Apply damage to the turret directly without resistance check
            turretHealth.TakeDamage(damage);

            // Check if the turret is destroyed and handle accordingly
            if (turretHealth.GetCurrentHealth() <= 0)
            {
                Debug.Log("Turret is destroyed!");
                Destroy(turret);
            }
        }
        else
        {
            Debug.LogError("No VariableComponent found on the turret!");
        }
    }

    // // Assuming you have a reference to the turret GameObject
    // enemyController.AttackTurret(turretGameObject, damageAmount); // 10f represents the damage amount


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
