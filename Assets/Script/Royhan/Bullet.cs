using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static float bulletDamage = 5;
    public static float rangeAttack = 0;
    [Header("Peluru ini akan hilang jika menabrak layer ini")]
    [SerializeField] private LayerMask objLayerCol;
    private SoundManager soundManager;

    private void Awake() 
    {
        soundManager = FindAnyObjectByType<SoundManager>();   
    }
    private void Update() 
    {
        // if (objLayerCol.Equals(other.gameObject.layer))
        // {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, rangeAttack, objLayerCol);
        foreach (var hitCollider in hitColliders)
        {
            int argh = Random.Range(0,1);
            if (hitCollider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log(hitCollider.gameObject.name);
                hitCollider.gameObject.GetComponent<VariableComponent>().TakeDamage(bulletDamage);
                gameObject.SetActive(false);
                soundManager.PlaySFX(23 + argh);
            }
            else
            {
                soundManager.PlaySFX(25 + argh);
                Debug.Log(hitCollider.tag);
                gameObject.SetActive(false);
            }
            Debug.Log("destroy");
        }
        // Destroy(gameObject);
        // }
    }
}
