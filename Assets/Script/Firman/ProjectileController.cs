using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public enum BulletType
    {
        Plasma,
        Bullet,
        Explosion
    }

    public BulletType bulletType; // Select the type of bullet in the inspector
    public ParticleSystem explosionVfx;
    public float speed = 20f;
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;
    public float impactDamage = 10f;
    private Transform target;

    private SoundManager soundManager;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
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
            HandleImpact();
        }
    }

    private void HandleImpact()
    {
        switch (bulletType)
        {
            case BulletType.Explosion:
                Explode();
                break;
            case BulletType.Plasma:
            case BulletType.Bullet:
                ImpactDamage();
                break;
        }
    }

    private void Explode()
    {
        // Deal damage to enemies within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            EnemyController enemy = hitCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage, bulletType);
                soundManager.PlaySFX(4);
            }
        }

        // Instantiate explosion VFX
        if (explosionVfx != null)
        {
            // Instantiate and destroy the VFX after 1 second
            ParticleSystem vfxInstance = Instantiate(explosionVfx, transform.position, Quaternion.identity);
            Destroy(vfxInstance.gameObject, 2f);
        }

        // Destroy the projectile after explosion
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
                enemy.TakeDamage(impactDamage, bulletType);
                break;
            }
        }

        Destroy(gameObject);
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, explosionRadius);
    // }
}
