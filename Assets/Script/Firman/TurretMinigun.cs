using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretMinigun : MonoBehaviour 
{
    public bool isMinigun = true;
    public float directionX = 0f;
    public float directionY = 0f;
    public float directionMinZ = 0f;
    public float directionMaxZ = 0f;
    public float detectionRadius = 10f;  
    public float detectionAngle = 60f;   
    public LayerMask enemyLayer;         
    
    public Transform turretHead;         
    public Transform[] firePoints;       // Array for 16 fire points
    public GameObject projectilePrefab;  

    public float initialFireCooldown = 0.8f;  // Initial delay between shots
    public float rapidFireDelay = 0.1f;        // Delay after 5 seconds
    private float fireCooldown;                 // Current fire cooldown

    private float elapsedTime = 0f;             // Time since started firing
    private Transform target;                    // Current target
    private float lastFireTime;                  // Last time a shot was fired
    private bool isFiring = false;               // State to prevent overlapping firing

    public enum TargetingMode
    {
        First,
        Strongest,
        Farthest
    }
    
    public TargetingMode targetingMode;  // Targeting mode variable

    // New arrays for firing points
    private Transform[] firstSetFirePoints;  // Array for fire points 1-7
    private Transform[] secondSetFirePoints; // Array for fire points 8-15
    private Transform[] notMinigunSetFirePoints; // Array for fire points 8-15


    void Start()
    {
        fireCooldown = initialFireCooldown;  // Set initial cooldown

        // Initialize fire point arrays
        if (isMinigun)
        {
            firstSetFirePoints = new Transform[8];  // 0-7
            secondSetFirePoints = new Transform[8]; // 8-15

            // Populate the arrays with the corresponding fire points
            for (int i = 0; i < 8; i++)
            {
                firstSetFirePoints[i] = firePoints[i];        // Fire points 0-7
                secondSetFirePoints[i] = firePoints[i + 8];  // Fire points 8-15
            }
        }
        else
        {
            notMinigunSetFirePoints =  new Transform[4];;
            for (int i = 0; i < 5; i++)
            {
                notMinigunSetFirePoints[i] = firePoints[i];
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;  // Increment elapsed time

        // Check for target and handle firing logic based on isMinigun
        target = FindTarget(); // Find a target each update

        if (target != null)
        {
            AimAtTarget();

            // If isMinigun is true, handle rapid firing
            if (isMinigun)
            {
                // Check if the elapsed time is greater than 5 seconds to switch to rapid fire
                if (elapsedTime > 5f && !isFiring)
                {
                    fireCooldown = rapidFireDelay;  // Change to rapid fire delay
                    StartCoroutine(FireFromFirePoints()); // Start firing coroutine
                }

                // Check if enough time has passed to shoot
                if (Time.time >= lastFireTime + fireCooldown && !isFiring)
                {
                    isFiring = true; // Set firing state
                    StartCoroutine(FireFromFirePoints()); // Start firing from fire points
                    lastFireTime = Time.time; 
                }
            }
            else // Normal firing behavior
            {
                if (Time.time >= lastFireTime + initialFireCooldown && !isFiring)
                {
                    isFiring = true; // Set firing state
                    StartCoroutine(FireFromNotMinigun()); // Start firing from the first 4 fire points
                }
            }
        }
        else
        {
            // If no target is found, reset firing state, cooldown, and elapsed time
            isFiring = false; 
            fireCooldown = initialFireCooldown; // Reset fire cooldown to initial
            elapsedTime = 0f; // Reset elapsed time
        }
    }
    IEnumerator FireFromNotMinigun()
    {
        for (int i = 0; i < 4; i++) // Only fire from the first 4 fire points
        {
            ShootAtTarget(firePoints[i]); // Fire from the current fire point
            lastFireTime = Time.time; // Update last fire time to the current time
            yield return new WaitForSeconds(initialFireCooldown); // Wait for the specified cooldown
        }
        isFiring = false; // Reset firing state after shooting all fire points
    }

    IEnumerator FireFromFirePoints()
    {
        // Fire from both sets of fire points simultaneously
        for (int i = 0; i < 8; i++)
        {
            // Fire from the first set (0 to 7)
            ShootAtTarget(firstSetFirePoints[i]);

            // Fire from the second set (8 to 15)
            ShootAtTarget(secondSetFirePoints[i]);

            // Wait for a short duration before the next shot
            yield return new WaitForSeconds(fireCooldown); // Wait for the specified fire cooldown
        }

        isFiring = false; // Reset firing state after shooting all fire points
    }

    Transform FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        List<Transform> validTargets = new List<Transform>();

        foreach (Collider col in colliders)
        {
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

        // Apply the Y rotation only to the turret head, maintaining its local rotation
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
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);
    }
}
