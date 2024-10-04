using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretLaser : MonoBehaviour
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
    public Transform firePoint;
    public GameObject projectilePrefab;
    public GameObject lastShootBeforePause;

    public float fireCooldown = 2f;
    public float fireDuration = 10f;
    public float pauseDuration = 4f;
    public float lastShoot = 2f;

    public enum TargetingMode { First, Strongest, Farthest }
    public TargetingMode targetingMode;

    private Transform target;
    private float lastFireTime;
    private bool isFiring = false;
    private bool canFire = true;

    void Update()
    {
        target = FindTarget();

        if (target != null && canFire && !isFiring)
        {

            StartCoroutine(FiringSequence());
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

        if (target == null)
            return;

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

        if (projectilePrefab == null || firePoint == null || target == null)
            return;


        Vector3 directionToTarget = target.position - firePoint.position;


        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);


        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, rotationToTarget);

        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            projectileController.SetTarget(target);
        }
    }


    IEnumerator FiringSequence()
    {

        yield return new WaitForSeconds(5f);

        isFiring = true;
        canFire = false;
        float fireStartTime = Time.time;

        while (Time.time < fireStartTime + fireDuration)
        {

            if (target == null)
            {
                target = FindTarget();
                if (target == null)
                    break;
            }

            AimAtTarget();

            if (Time.time >= lastFireTime + fireCooldown)
            {
                ShootAtTarget();
                lastFireTime = Time.time;
            }

            yield return null;
        }


        yield return new WaitForSeconds(lastShoot);


        if (target != null && lastShootBeforePause != null)
        {
            AimAtTarget();
            GameObject lastShootProjectile = Instantiate(lastShootBeforePause, firePoint.position, firePoint.rotation);
            ProjectileController projectileController = lastShootProjectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.SetTarget(target);
            }
        }


        isFiring = false;
        yield return new WaitForSeconds(pauseDuration);


        canFire = true;
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
