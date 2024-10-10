using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private GameObject bulletPref;
    public PlayerMovement playerMovement;
    // [SerializeField] private float delay = 0.1f;
    private void OnEnable() 
    {
        if (playerMovement != null)
        {
            playerMovement.bulletDamage = damage;
            playerMovement.bulletPrefab = bulletPref;
        }
    }

}
