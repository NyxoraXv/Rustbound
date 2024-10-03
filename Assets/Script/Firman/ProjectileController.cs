using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 20f;
    public bool isExplode = false;
    public float explosionRadius = 5f; 
    public float explosionDamage = 50f; 
    public float impactDamage = 10f; 
    private Transform target;

    
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);  
            return;
        }

        
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        
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

    
    private void Explode()
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            
            EnemyController enemy = hitCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        
        Destroy(gameObject); 
    }

    private void ImpactDamage()
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (var hitCollider in hitColliders)
        {
            EnemyController enemy = hitCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(impactDamage);
                break; 
            }
        }
        
        Destroy(gameObject); 
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, explosionRadius); 
    }
}
