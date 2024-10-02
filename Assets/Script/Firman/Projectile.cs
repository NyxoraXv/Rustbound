using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;       // Speed of the projectile.
    private Transform target;       // The target the projectile is moving towards.

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);     // Destroy the projectile if there's no target.
            return;
        }

        // Move towards the target.
        Vector3 direction = (target.position - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        // Check if the projectile would hit the target this frame.
        if (Vector3.Distance(transform.position, target.position) <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        // Move the projectile towards the target.
        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        // Logic for hitting the target (like playing an effect) can go here.

        // Destroy the projectile after reaching the target.
        Destroy(gameObject);
    }
}
