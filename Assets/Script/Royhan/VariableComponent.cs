using UnityEngine;
using UnityEngine.Events;

public class VariableComponent : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float maxHealth;
    public float _currentHealth { get; protected set; }

    // UnityEvent for taking damage
    public UnityEvent<float> OnTakeDamage; // Pass the amount of damage taken as a float parameter

    private void Start()
    {
        _currentHealth = maxHealth;

        // Initialize the event if it has no listeners
        if (OnTakeDamage == null)
        {
            OnTakeDamage = new UnityEvent<float>();
        }
    }

    // Method to take damage.
    public virtual void TakeDamage(float damage)
    {
        Debug.Log("damaging");
        Debug.Log(Bullet.bulletDamage);
        _currentHealth -= damage;

        // Trigger the damage event
        OnTakeDamage.Invoke(damage);

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
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        _currentHealth = maxHealth;
        Debug.Log($"Max health increased to: {maxHealth}");
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
