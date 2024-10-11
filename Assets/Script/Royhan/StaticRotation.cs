using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        // Simpan rotasi awal
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // Kunci rotasi child sesuai rotasi awal
        transform.rotation = initialRotation;
    }
}
