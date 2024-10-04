using UnityEngine;

public class VariableComponent : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float maxHealth;
    public float _currentHealth { get; private set; }

    private void Start() 
    {
        _currentHealth = maxHealth;    
    }

    // Method to take damage.
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    // Method to retrieve current health.
    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    // Method to retrieve max health.
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        // Optionally, reset the current health to max if desired
        _currentHealth = maxHealth;
        Debug.Log($"Max health increased to: {maxHealth}");
    }


    // Handle the enemy death.
    private void Die()
    {
        // Add death logic here (e.g., play an animation, drop loot, etc.)
        Destroy(gameObject);  // Destroy the object when it dies.
    }
}
