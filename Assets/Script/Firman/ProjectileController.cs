using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 20f;
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

        // Destroy the projectile when it reaches the target.
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            Destroy(gameObject);  // You could add explosion effects here.
        }
    }
}
