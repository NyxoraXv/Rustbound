using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TurretDoubleType : VariableComponent, ITurret
{
    public Image healthBar;
    [Header("Max Spawn Turret")]
    public bool isFollowTuretHead = true;
    public float directionX = 0f;
    public float directionY = 0f;
    public float directionMinZ = 0f;
    public float directionMaxZ = 0f;
    public float detectionRadius = 10f;
    public float detectionAngle = 60f;
    public LayerMask enemyLayer;

    public Transform turretHead;
    public Transform firePoint1;
    public Transform firePoint2;
    public GameObject projectilePrefab1;
    public GameObject projectilePrefab2;

    public ParticleSystem particle;
    public float fireCooldown = 2f;

    public enum TargetingMode { First, Strongest, Farthest }
    public TargetingMode targetingMode;

    [Header("Explosion After Dead")]
    public ParticleSystem explosionVFX;

    private Transform target;
    private float lastFireTime;

    private float targetUpdateInterval = 1f; // Update target every second
    private float nextTargetUpdateTime = 0f;

    private bool isPreviewObject;
    private SoundManager soundManager;

    void Start()
    {
        _currentHealth = maxHealth;

        soundManager = FindObjectOfType<SoundManager>();
    }

    void Update()
    {
        if (Time.time >= nextTargetUpdateTime)
        {
            target = FindTarget();
            nextTargetUpdateTime = Time.time + targetUpdateInterval; // Schedule next update
        }

        if (target != null)
        {
            AimAtTarget();

            if (Time.time >= lastFireTime + fireCooldown)
            {
                ShootAtTarget();
                Debug.Log("Fire");
                lastFireTime = Time.time;
            }
        }
        float healthPercentage =_currentHealth / maxHealth;; 
        if (healthPercentage<1)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.fillAmount = healthPercentage;
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    Transform FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        List<Transform> validTargets = new List<Transform>();

        foreach (Collider col in colliders)
        {

            VariableComponent variableComponent = col.GetComponent<VariableComponent>();
            if (variableComponent == null) continue;

            Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget <= detectionAngle / 2f)
            {
                validTargets.Add(col.transform);
            }
        }

        if (validTargets.Count > 0)
        {
            switch (targetingMode)
            {
                case TargetingMode.First:
                    validTargets.Sort((a, b) => Vector3.Distance(turretHead.position, a.position)
                        .CompareTo(Vector3.Distance(turretHead.position, b.position)));
                    break;

                case TargetingMode.Strongest:
                    validTargets.Sort((a, b) => b.GetComponent<VariableComponent>().maxHealth
                        .CompareTo(a.GetComponent<VariableComponent>().maxHealth));
                    break;

                case TargetingMode.Farthest:
                    validTargets.Sort((a, b) => Vector3.Distance(turretHead.position, b.position)
                        .CompareTo(Vector3.Distance(turretHead.position, a.position)));
                    break;
            }

            return validTargets[0];
        }

        return null;
    }


    void AimAtTarget()
    {
        Vector3 direction = (target.position - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        Vector3 euler = lookRotation.eulerAngles;
        euler.x = directionX;
        euler.z = Mathf.Clamp(euler.z, directionMinZ, directionMaxZ);


        euler.y -= directionY;

        turretHead.DORotate(euler, 0.5f);


        if (isFollowTuretHead)
        {
            firePoint1.rotation = turretHead.rotation;
            firePoint2.rotation = turretHead.rotation;
        }
    }

    void ShootAtTarget()
    {
        if (projectilePrefab1 != null && firePoint1 != null)
        {

            GameObject projectile1 = Instantiate(projectilePrefab1, firePoint1.position, firePoint1.rotation);
            ProjectileController projectileController1 = projectile1.GetComponent<ProjectileController>();
            if (projectileController1 != null)
            {
                projectileController1.SetTarget(target);
                soundManager.PlaySFX(0);
            }
        }


        Invoke("ShootFromFirePoint2", 0.2f);
    }

    void ShootFromFirePoint2()
    {
        if (projectilePrefab2 != null && firePoint2 != null)
        {

            GameObject projectile2 = Instantiate(projectilePrefab2, firePoint2.position, firePoint2.rotation);
            particle.Play();
            ProjectileController projectileController2 = projectile2.GetComponent<ProjectileController>();
            if (projectileController2 != null)
            {
                projectileController2.SetTarget(target);
                soundManager.PlaySFX(3);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        int[] sfxOptions = { 6, 7, 8 };

        // Pick a random SFX
        int randomIndex = Random.Range(0, sfxOptions.Length);
        int randomSFX = sfxOptions[randomIndex];

        // Play the randomly selected SFX
        soundManager.PlaySFX(randomSFX);

        if (GetCurrentHealth() <= 0)
        {
            DestroyTurret();
        }
    }

    private void DestroyTurret()
    {
        if (explosionVFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance.gameObject, 4f);
        }
        PlacementSystem.Instance.RemoveObject(1);
        soundManager.PlaySFX(5);
        Destroy(gameObject); // Destroy the turret GameObject
    }

    public void SetIsPreviewObject(bool isPreview)
    {
        isPreviewObject = isPreview; // Method to set the preview object flag
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, detectionRadius);

    //     Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
    //     Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;

    //     Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
    //     Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);
    // }
}
