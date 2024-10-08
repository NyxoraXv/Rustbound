using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Peluru ini akan hilang jika menabrak layer ini")]
    [SerializeField] private LayerMask objLayerCol;
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.layer == objLayerCol)
        {
            Destroy(gameObject);
        }
    }
}
