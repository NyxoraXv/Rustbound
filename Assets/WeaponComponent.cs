using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [HideInInspector] public PlayerMovement playerMovement;
    // [SerializeField] private float delay = 0.1f;
    private void OnEnable() 
    {
        playerMovement.bulletDamage = damage;
    }

}
