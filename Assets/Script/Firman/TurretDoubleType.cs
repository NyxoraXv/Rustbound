using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretDoubleType : MonoBehaviour
{
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
    public GameObject projectilePrefab1; // Projectile for firePoint1
    public GameObject projectilePrefab2; // Projectile for firePoint2

    public float fireCooldown = 2f;

    public enum TargetingMode { First, Strongest, Farthest }
    public TargetingMode targetingMode;

    private Transform target;
    private float lastFireTime;

    void Update()
    {
        target = FindTarget();

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
            // Check if VariableComponent exists and is valid
            VariableComponent variableComponent = col.GetComponent<VariableComponent>();
            if (variableComponent == null) continue; // Skip if component is not found

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

        return null; // Return null if no valid targets found
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

        // Optional: If firePoint is a child of turretHead, it will follow automatically
        if (isFollowTuretHead)
        {
            firePoint1.rotation = turretHead.rotation;
            firePoint2.rotation = turretHead.rotation; // Update firePoint2's rotation as well
        }
    }

    void ShootAtTarget()
    {
        if (projectilePrefab1 != null && firePoint1 != null)
        {
            // Shoot from firePoint1
            GameObject projectile1 = Instantiate(projectilePrefab1, firePoint1.position, firePoint1.rotation);
            ProjectileController projectileController1 = projectile1.GetComponent<ProjectileController>();
            if (projectileController1 != null)
            {
                projectileController1.SetTarget(target);
            }
        }

        // Wait a short duration before firing from firePoint2
        Invoke("ShootFromFirePoint2", 0.2f); // Adjust the delay as needed
    }

    void ShootFromFirePoint2()
    {
        if (projectilePrefab2 != null && firePoint2 != null)
        {
            // Shoot from firePoint2
            GameObject projectile2 = Instantiate(projectilePrefab2, firePoint2.position, firePoint2.rotation);
            ProjectileController projectileController2 = projectile2.GetComponent<ProjectileController>();
            if (projectileController2 != null)
            {
                projectileController2.SetTarget(target);
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
