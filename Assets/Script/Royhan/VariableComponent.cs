using UnityEngine;

public class VariableComponent : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float maxHealth;
    public float _currentHealth { get; protected set; }

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
    
    public void Heal(float amount)
    {
        _currentHealth += amount; // Assume currentHealth is a float variable
        // Clamp the health to the maximum health value if needed
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth); // Assuming you have maxHealth defined
    }

    
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        // Optionally, reset the current health to max if desired
        _currentHealth = maxHealth;
        Debug.Log($"Max health increased to: {maxHealth}");
    }


    // Handle the enemy death.
    protected virtual void Die()
    {
        // Add death logic here (e.g., play an animation, drop loot, etc.)
        Destroy(gameObject);  // Destroy the object when it dies.
    }
}
