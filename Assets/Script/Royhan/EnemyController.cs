using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private VariableComponent variableComponent;

    private void Start()
    {
        // Get the VariableComponent attached to this GameObject
        variableComponent = GetComponent<VariableComponent>();
        if (variableComponent == null)
        {
            Debug.LogError("VariableComponent not found on this GameObject.");
        }
    }

    // Method to take damage
    public void TakeDamage(float damage)
    {
        if (variableComponent != null)
        {
            variableComponent.TakeDamage(damage);
            // Optional: Add visual feedback, sound, or other effects here
            Debug.Log($"Enemy took {damage} damage! Current health: {variableComponent.GetCurrentHealth()}");

            // Optional: Check if the enemy is dead and handle accordingly
            if (!IsAlive())
            {
                Debug.Log("Enemy is dead!");
            }
        }
    }

    // Optional: Method to check if the enemy is alive
    public bool IsAlive()
    {
        return variableComponent != null && variableComponent.GetCurrentHealth() > 0;
    }
}
