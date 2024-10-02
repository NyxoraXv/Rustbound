using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretCone : MonoBehaviour
{
    public float detectionRadius = 10f;  // The radius of detection.
    public float detectionAngle = 60f;   // The angle of the detection cone.
    public LayerMask enemyLayer;         // Layer mask to filter out enemies.

    public Transform turretHead;         // The part of the turret that rotates to aim.
    public Transform firePoint;          // The point from which the turret shoots.
    public GameObject projectilePrefab;  // The projectile prefab to shoot.

    public enum TargetingMode { First, Strongest, Farthest }
    public TargetingMode targetingMode;  // The current targeting mode of the turret.

    private Transform target;

    void Update()
    {
        // Find a target based on the targeting mode.
        target = FindTarget();

        // If a target is found, rotate towards the target and shoot.
        if (target != null)
        {
            AimAtTarget();
            ShootAtTarget();
        }
    }

    // Function to find the closest enemy based on the targeting mode.
    Transform FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        List<Transform> validTargets = new List<Transform>();

        foreach (Collider col in colliders)
        {
            Vector3 directionToTarget = (col.transform.position - transform.position).normalized;

            // Check if the target is within the detection angle.
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            if (angleToTarget <= detectionAngle / 2f)
            {
                validTargets.Add(col.transform);
            }
        }

        // If there are valid targets, sort them based on the targeting mode.
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

            return validTargets[0];  // Return the best target based on the sort.
        }

        return null;  // No target found.
    }

    void AimAtTarget()
    {
        Vector3 direction = (target.position - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Lock the X rotation at -45 and clamp the Z rotation between 70 and 90
        Vector3 euler = lookRotation.eulerAngles;
        euler.x = -45f; // Fix X rotation at -45
        euler.z = Mathf.Clamp(euler.z, 80f, 90f); // Restrict Z rotation

        // Start Y rotation from -90
        euler.y -= 90f;

        // Apply rotation using DoTween
        turretHead.DORotate(euler, 1f); // Adjust time for smoother rotation if needed
    }


    // Shoot at the target.
    void ShootAtTarget()
    {
        // Example shooting mechanism: Instantiate a projectile.
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.SetTarget(target);  // Assign the target to the projectile.
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
