using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 20f;
    public bool isExplode = false;
    public float explosionRadius = 5f; // Radius for explosion damage
    public float explosionDamage = 50f; // Damage dealt to enemies upon explosion
    public float impactDamage = 10f; // Damage dealt to enemies on impact when not exploding
    private Transform target;

    // Set the target for the projectile.
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);  // Destroy the projectile if the target is null.
            return;
        }

        // Move the projectile towards the target.
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Destroy the projectile when it reaches the target or if it's set to explode.
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            if (isExplode)
            {
                Explode();
            }
            else
            {
                ImpactDamage();
            }
        }
    }

    // Method to handle explosion logic
    private void Explode()
    {
        // Find enemies within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            // Assuming enemies have a script with a method to take damage
            EnemyController enemy = hitCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        // You can also add explosion effects here, such as particle systems or sound
        Destroy(gameObject); // Destroy the projectile after exploding
    }

    private void ImpactDamage()
    {
        // Check for enemies within a small radius around the projectile's position
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (var hitCollider in hitColliders)
        {
            EnemyController enemy = hitCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(impactDamage);
                break; // Exit loop after applying damage to one enemy
            }
        }
        
        Destroy(gameObject); // Destroy the projectile after applying impact damage
    }

    // Draw the explosion radius in the scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set the color of the Gizmos
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Draw the wire sphere
    }
}
