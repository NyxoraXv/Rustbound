using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretAdvance : VariableComponent, ITurret
{
    [Header ("Max Spawn Turret")]
    public bool isMinigun = true;
    public float directionX = 0f;
    public float directionY = 0f;
    public float directionMinZ = 0f;
    public float directionMaxZ = 0f;
    public float detectionRadius = 10f;
    public float detectionAngle = 60f;
    public LayerMask enemyLayer;

    public Transform turretHead;
    public Transform[] firePoints;
    public GameObject projectilePrefab;

    public float initialFireCooldown = 0.8f;
    public float rapidFireDelay = 0.1f;
    public ParticleSystem gunEffect;
    private float fireCooldown;

    private float elapsedTime = 0f;
    private Transform target;
    private float lastFireTime;
    private bool isFiring = false;

    public enum TargetingMode
    {
        First,
        Strongest,
        Farthest
    }

    public TargetingMode targetingMode;

    [Header("Explosion After Dead")]
    public ParticleSystem explosionVFX;

    private Transform[] firstSetFirePoints;
    private Transform[] secondSetFirePoints;
    private Transform[] notMinigunSetFirePoints;
    private float targetUpdateInterval = 1f; // Update target every second
    private float nextTargetUpdateTime = 0f;
    private bool isPreviewObject;

    private SoundManager soundManager;

    void Start()
    {
        fireCooldown = initialFireCooldown;


        if (isMinigun)
        {
            firstSetFirePoints = new Transform[8];
            secondSetFirePoints = new Transform[8];


            for (int i = 0; i < 8; i++)
            {
                firstSetFirePoints[i] = firePoints[i];
                secondSetFirePoints[i] = firePoints[i + 8];
            }
        }
        else
        {
            notMinigunSetFirePoints = new Transform[5];
            for (int i = 0; i < 5; i++)
            {
                notMinigunSetFirePoints[i] = firePoints[i];
            }
        }

        _currentHealth = maxHealth;

        soundManager = FindAnyObjectByType<SoundManager>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;


        if (Time.time >= nextTargetUpdateTime)
        {
            target = FindTarget();
            nextTargetUpdateTime = Time.time + targetUpdateInterval; // Schedule next update
        }

        if (target != null)
        {
            AimAtTarget();


            if (isMinigun)
            {

                if (elapsedTime > 5f && !isFiring)
                {
                    fireCooldown = rapidFireDelay;
                    StartCoroutine(FireFromFirePoints());
                }


                if (Time.time >= lastFireTime + fireCooldown && !isFiring)
                {
                    isFiring = true;
                    StartCoroutine(FireFromFirePoints());
                    lastFireTime = Time.time;
                }
            }
            else
            {
                if (Time.time >= lastFireTime + initialFireCooldown && !isFiring)
                {
                    isFiring = true;
                    StartCoroutine(FireFromNotMinigun());
                }
            }
        }
        else
        {

            isFiring = false;
            fireCooldown = initialFireCooldown;
            elapsedTime = 0f;
        }
    }
    IEnumerator FireFromNotMinigun()
    {
        for (int i = 0; i < 5; i++)
        {
            ShootAtTarget(firePoints[i]);
            gunEffect.Play();
            lastFireTime = Time.time;
            yield return new WaitForSeconds(initialFireCooldown);
        }
        isFiring = false;
    }

    IEnumerator FireFromFirePoints()
    {

        for (int i = 0; i < 8; i++)
        {

            ShootAtTarget(firstSetFirePoints[i]);
            gunEffect.Play();

            ShootAtTarget(secondSetFirePoints[i]);


            yield return new WaitForSeconds(fireCooldown);
        }

        isFiring = false;
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
    }

    void ShootAtTarget(Transform firePoint)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.SetTarget(target);
                soundManager.PlaySFX(0);
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
        PlacementSystem.Instance.RemoveObject(8);
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
