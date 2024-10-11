using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum ShootingType
{
    Normal,
    Spread,
    Missile
}

public class WeaponHandler : MonoBehaviour
{
    [Header("ID")]
    public int ID;

    [Header("Weapon Stats")]
    public int weaponDamage;               // Damage dealt by the weapon
    public float weaponRateOfFire;         // Rate of fire in shots per second
    [Range(0, 1.5f)]
    public float weaponAccuracy;           // Weapon accuracy, higher values mean less spread

    [Header("Weapon Attributes")]
    public string weaponName;              // Name of the weapon
    public float reloadTime;               // Time to reload the weapon
    public int magazineSize;               // Number of bullets per magazine
    public int currentAmmo;                // Current bullets in the magazine
    public float shootForce = 5f;          // Bullet force
    public ShootingType shootingType;      // Type of shooting (Normal, Spread, Missile)

    [Header("References")]
    public GameObject bulletPrefab;        // Bullet prefab
    public GameObject missilePrefab;       // Missile prefab (for Missile shoot type)
    public GameObject particleEffectPrefab;// Particle effect prefab for shooting
    public Transform shootPos;             // Shoot position
    public LayerMask layerRaycast;         // Layer for raycasting targets

    private bool isReloading = false;
    private Camera mainCamera;
    private List<GameObject> bulletPool;   // Pool of bullets
    public int poolSize = 10;              // Bullet pool size
    public int spreadAmount = 5;           // For Spread shooting (e.g., shotgun pellets)

    public int currentSlot;

    public void init(WeaponData weaponData, int slot)
    {
        weaponName = weaponData.weaponName;
        ID = weaponData.weaponID;
        weaponDamage = weaponData.weaponDamage;
        weaponRateOfFire = weaponData.weaponRateOfFire;
        weaponAccuracy = weaponData.weaponAccuracy;
        reloadTime = weaponData.reloadTime;
        magazineSize = weaponData.magazineSize;
        currentAmmo = weaponData.currentAmmo;
        shootForce = weaponData.shootForce;
        shootingType = weaponData.shootingType;
        currentSlot = slot;
        // You can also instantiate the weaponPrefab if needed or manage UI here
        // For example:
        // GameObject instantiatedWeapon = Instantiate(weaponData.weaponPrefab, somePosition, someRotation);
        // Instantiate the weapon's image or prefab here if needed
    }


    private void Awake()
    {
        currentAmmo = magazineSize;

        // Initialize bullet pool
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            bulletPool.Add(obj);
        }

        mainCamera = Camera.main;
    }

    // Method to fire the weapon
    public void Fire()
    {
        Debug.Log("Firing 1" );
        if (currentAmmo > 0 && !isReloading)
        {
            Debug.Log("Firing" );
            currentAmmo--;

            // Handle different shooting types
            switch (shootingType)
            {
                case ShootingType.Normal:
                    ShootNormal();
                    break;
                case ShootingType.Spread:
                    ShootSpread();
                    break;
                case ShootingType.Missile:
                    ShootMissile();
                    break;
            }

            if (currentAmmo == 0)
            {
                StartCoroutine(Reload());
            }
        }
        else if (currentAmmo == 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    // Normal shooting method
    private void ShootNormal()
    {
        GameObject bullet = GetPooledBullet();
        if (bullet != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerRaycast))
            {
                Vector3 direction = (hit.point - shootPos.position).normalized;

                bullet.transform.position = shootPos.position;
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                bullet.SetActive(true);
                bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
                bullet.GetComponent<Rigidbody>().AddForce(direction * shootForce, ForceMode.Impulse);

                Instantiate(particleEffectPrefab, shootPos.position, Quaternion.identity); // Particle effect

                StartCoroutine(DisableBulletAfterTime(bullet, 4f));
            }
        }
    }

    // Spread shooting method (like a shotgun)
    private void ShootSpread()
    {
        for (int i = 0; i < spreadAmount; i++)
        {
            GameObject bullet = GetPooledBullet();
            if (bullet != null)
            {
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerRaycast))
                {
                    Vector3 direction = (hit.point - shootPos.position).normalized;

                    // Randomize the spread angle for each pellet
                    direction = Quaternion.Euler(
                        UnityEngine.Random.Range(-weaponAccuracy, weaponAccuracy),
                        UnityEngine.Random.Range(-weaponAccuracy, weaponAccuracy),
                        0) * direction;

                    bullet.transform.position = shootPos.position;
                    bullet.transform.rotation = Quaternion.LookRotation(direction);
                    bullet.SetActive(true);
                    bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    bullet.GetComponent<Rigidbody>().AddForce(direction * shootForce, ForceMode.Impulse);

                    StartCoroutine(DisableBulletAfterTime(bullet, 4f));
                }
            }
        }
        Instantiate(particleEffectPrefab, shootPos.position, Quaternion.identity); // Particle effect
    }

    // Missile shooting method
    private void ShootMissile()
    {
        GameObject missile = Instantiate(missilePrefab, shootPos.position, shootPos.rotation);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerRaycast))
        {
            Vector3 direction = (hit.point - shootPos.position).normalized;
            missile.GetComponent<Rigidbody>().velocity = direction * shootForce;
        }

        Instantiate(particleEffectPrefab, shootPos.position, Quaternion.identity); // Particle effect
    }

    // Method to handle bullet pooling
    private GameObject GetPooledBullet()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null;
    }

    // Coroutine to disable bullet after a delay
    private IEnumerator DisableBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);
    }

    // Coroutine to reload the weapon
    private IEnumerator Reload()
    {
        if (!isReloading)
        {
            isReloading = true;
            Debug.Log("Reloading...");

            yield return new WaitForSeconds(reloadTime);

            currentAmmo = magazineSize;
            isReloading = false;
        }
    }
}
