using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretCone : MonoBehaviour
{
    public float detectionRadius = 10f;      // The radius of detection.
    public float detectionAngle = 45f;       // The angle of the detection cone.
    public LayerMask enemyLayer;             // Layer mask to filter out enemies.

    public Transform turretHead;             // The part of the turret that rotates to aim.
    public Transform firePoint;              // The point from which the turret shoots.
    public GameObject projectilePrefab;      // The projectile prefab to shoot.

    public float rotationDuration = 0.5f;    // Duration for DoTween rotation.
    public float fireDelay = 1.8f;           // Delay between each shot.
    private float lastFireTime = 0f;         // Tracks when the turret last fired.

    private float minZRotation = 70f;        // Minimum Z rotation angle.
    private float maxZRotation = 90f;        // Maximum Z rotation angle.

    private Transform target;

    void Update()
    {
        // Find a target within the cone radius.
        target = FindTargetInCone();

        // If a target is found, rotate towards the target.
        if (target != null)
        {
            AimAtTarget();

            // If enough time has passed since the last shot, shoot at the target.
            if (Time.time >= lastFireTime + fireDelay)
            {
                ShootAtTarget();
                lastFireTime = Time.time;  // Update the last fire time.
            }
        }
    }

    // Function to find the closest enemy within the cone radius.
    Transform FindTargetInCone()
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

        // Return the closest target if there is any.
        if (validTargets.Count > 0)
        {
            validTargets.Sort((a, b) =>
                Vector3.Distance(transform.position, a.position).CompareTo(
                Vector3.Distance(transform.position, b.position))
            );

            return validTargets[0];
        }

        return null;  // No target found within the cone.
    }

    // Rotate the turret towards the target, with clamped Z-axis rotation and Y rotation starting from -90 degrees.
    void AimAtTarget()
    {
        Vector3 direction = (target.position - turretHead.position).normalized;

        // Calculate the look rotation, but only apply it to the Y and Z axes.
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float currentZRotation = Mathf.Clamp(lookRotation.eulerAngles.z, minZRotation, maxZRotation);

        // Adjust the Y-axis rotation to start from -90 degrees.
        float adjustedYRotation = lookRotation.eulerAngles.y - 90f;

        // Create the desired rotation, locking X-axis and clamping Z-axis.
        Quaternion targetRotation = Quaternion.Euler(
            turretHead.rotation.eulerAngles.x, // Keep the current X rotation.
            adjustedYRotation,                 // Start Y rotation from -90 degrees.
            currentZRotation                   // Clamp Z rotation.
        );

        // Use DoTween to smoothly rotate the turret to the target rotation.
        turretHead.DORotateQuaternion(targetRotation, rotationDuration);
    }

    // Shoot at the target.
    void ShootAtTarget()
    {
        // Example shooting mechanism: Instantiate a projectile.
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            // Pass the target's position to the projectile.
            if (projectileScript != null)
            {
                projectileScript.SetTarget(target);
            }
        }
    }

    // Optionally, visualize the detection cone in the scene view.
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
