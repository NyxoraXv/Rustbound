using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [Header("Line Renderer Components")]
    public LineRenderer lineRenderer;
    public Transform startPos;
    public Transform laserOrigin;  // Titik asal laser
    public float maxLaserDistance = 100f;  // Jarak maksimum laser
    public LayerMask collisionMask;  // Mask untuk layer yang ingin dideteksi

    private void Start()
    {
        // Pastikan Line Renderer memiliki 2 posisi (awal dan akhir)
        lineRenderer.positionCount = 2;

        // Set posisi awal Line Renderer
        lineRenderer.SetPosition(0, startPos.position);
    }

    private void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
    {
        // Mulai raycast dari posisi asal laser ke arah depan
        RaycastHit hit;
        float laserDistance = maxLaserDistance;  // Default ke jarak maksimum

        // Lakukan raycast untuk mendeteksi objek di depan
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, maxLaserDistance, collisionMask))
        {
            // Jika ada tabrakan, set panjang laser sesuai dengan jarak ke objek yang ditabrak
            laserDistance = hit.distance;

            // Optionally: Bisa lakukan sesuatu pada objek yang terkena laser
            Debug.Log("Laser hit: " + hit.collider.name);
        }

        // Set posisi awal di (0,0,0) dan posisi akhir hanya mengubah z axis
        // lineRenderer.SetPosition(0, startPos.position);
        lineRenderer.SetPosition(1, new Vector3(0f, transform.parent.transform.position.y, laserDistance));
    }
} 
