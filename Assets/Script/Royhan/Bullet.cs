using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Peluru ini akan hilang jika menabrak layer ini")]
    [SerializeField] private float bulletDamage = 5;
    [SerializeField] private LayerMask objLayerCol;
    [SerializeField] private Rigidbody _rigidbody;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody>();     
    }
    private void OnCollisionEnter(Collision other) 
    {
        // if (objLayerCol.Equals(other.gameObject.layer))
        // {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<VariableComponent>().TakeDamage(bulletDamage);
        }
        else
        {
            gameObject.SetActive(false);

        }
        Debug.Log("destroy");
        // Destroy(gameObject);
        // }
    }
}
