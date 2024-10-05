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

    public float damageDealtToTurret = 10f;
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

    public void AttackTurret(GameObject turret)
    {
        // Try to get the VariableComponent on the turret
        VariableComponent turretHealth = turret.GetComponent<VariableComponent>();
        if (turretHealth != null)
        {
            // Apply damage to the turret directly
            turretHealth.TakeDamage(damageDealtToTurret);
            Debug.Log($"{turret.name} attacked! Damage dealt: {damageDealtToTurret}");

            // Optional: Check if the turret is destroyed
            if (turretHealth.GetCurrentHealth() <= 0)
            {
                Debug.Log($"{turret.name} is destroyed!");
            }
        }
        else
        {
            Debug.LogError("No VariableComponent found on the turret!");
        }
    }

    // Optional: Example method to demonstrate attacking a turret
    public void AttackTurretExample()
    {
        GameObject turret = GameObject.FindWithTag("Turret"); // Find turret by tag
        if (turret != null)
        {
            AttackTurret(turret);
        }
        else
        {
            Debug.LogError("No turret found with the specified tag!");
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
