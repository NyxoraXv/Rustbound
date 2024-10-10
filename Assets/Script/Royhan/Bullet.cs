using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletDamage = 5;
    [Header("Peluru ini akan hilang jika menabrak layer ini")]
    [SerializeField] private LayerMask objLayerCol;
    private Rigidbody _rigidbody;

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
            gameObject.SetActive(false);
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
