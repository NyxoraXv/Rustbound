using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretCone : MonoBehaviour, ITurret
{
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
    public Transform firePoint;
    public GameObject projectilePrefab;

    public float fireCooldown = 2f;

    public enum TargetingMode { First, Strongest, Farthest }
    public TargetingMode targetingMode;

    [Header("Explosion After Dead")]
    public ParticleSystem explosionVFX;

    private Transform target;
    private float lastFireTime;

    private VariableComponent variableComponent;
    private float targetUpdateInterval = 1f; // Update target every second
    private float nextTargetUpdateTime = 0f;
    private bool isPreviewObject;
    private SoundManager soundManager;

    void Start()
    {
        // Initialize the VariableComponent if it's on the same GameObject
        variableComponent = GetComponent<VariableComponent>();

        // Optionally, log an error if the component is missing
        if (variableComponent == null)
        {
            Debug.LogError("VariableComponent is missing from the Turret GameObject!");
        }

        soundManager = FindAnyObjectByType<SoundManager>();
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
                lastFireTime = Time.time;
            }
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
            firePoint.rotation = turretHead.rotation;
        }
    }

    void ShootAtTarget()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.SetTarget(target);
                soundManager.PlaySFX(3);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (variableComponent != null)
        {
            variableComponent.TakeDamage(damage);

            int[] sfxOptions = { 6, 7, 8 };

            // Pick a random SFX
            int randomIndex = Random.Range(0, sfxOptions.Length);
            int randomSFX = sfxOptions[randomIndex];

            // Play the randomly selected SFX
            soundManager.PlaySFX(randomSFX);
            
            Debug.Log($"TurretCone took damage: {damage}, Current Health: {variableComponent.GetCurrentHealth()}");

            if (variableComponent.GetCurrentHealth() <= 0)
            {
                DestroyTurret();
            }
        }
    }

    private void DestroyTurret()
    {
        Debug.Log("Turret destroyed!");
        if (explosionVFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance.gameObject, 4f);
        }
        soundManager.PlaySFX(5);
        Destroy(gameObject); // Destroy the turret GameObject
    }

    private void OnDestroy()
    {
        if (!isPreviewObject)
        {
            DestroyTurret(); // Call DestroyTurret if it's not a preview object
        }
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
