using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static float bulletDamage = 5;
    public static float rangeAttack = 0;
    [Header("Peluru ini akan hilang jika menabrak layer ini")]
    [SerializeField] private LayerMask objLayerCol;
    private Rigidbody _rigidbody;

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody>();     
    }
    private void Update() 
    {
        // if (objLayerCol.Equals(other.gameObject.layer))
        // {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, rangeAttack, objLayerCol);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log(hitCollider.gameObject.name);
                hitCollider.gameObject.GetComponent<VariableComponent>().TakeDamage(bulletDamage);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log(hitCollider.tag);
                gameObject.SetActive(false);
            }
            Debug.Log("destroy");
        }
        // Destroy(gameObject);
        // }
    }
}
