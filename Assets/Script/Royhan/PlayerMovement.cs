using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootForce = 5f;
    [SerializeField] private int poolSize = 10; // Ukuran pool
    private float speed = 5f;
    private float rotationSpeed = 5f;
    private Transform cameraTransform; 

    private Vector3 moveDirection;
    private Vector2 _movementInput;
    private Rigidbody _rigidbody;
    private VariableComponent variableComponent;

    private List<GameObject> bulletPool; // Pool untuk peluru
    private Camera mainCamera; // Untuk mengambil posisi mouse

    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody>();
        variableComponent = GetComponent<VariableComponent>();
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        mainCamera = Camera.main;

        speed = variableComponent.speed;
        rotationSpeed = variableComponent.rotationSpeed;

        // Inisialisasi pooling
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            bulletPool.Add(obj);
        }
    }

    private void FixedUpdate() 
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * _movementInput.y + right * _movementInput.x;
        _rigidbody.velocity = new Vector3(moveDirection.x * speed, _rigidbody.velocity.y, moveDirection.z * speed);

        HandleRotation(moveDirection);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
        
        if (_movementInput != Vector2.zero && !_rigidbody.freezeRotation)
        {
            _rigidbody.freezeRotation = true;
        }

        if (_movementInput.magnitude < 0.1f)
        {
            _movementInput = Vector2.zero;
        }
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        Debug.Log("forward");
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject bulletPush = GetPooledBullet();
            if (bulletPush != null)
            {
                // Menentukan arah tembakan berdasarkan posisi mouse
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                     Vector3 direction = (hit.point - shootPos.position).normalized;
                    bulletPush.transform.position = shootPos.position;

                    // Hanya ubah rotasi pada sumbu Y
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    bulletPush.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                
                    bulletPush.SetActive(true);
                    bulletPush.GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset velocity sebelum menembak
                    bulletPush.GetComponent<Rigidbody>().AddForce(direction * shootForce, ForceMode.Impulse);
                    
                    StartCoroutine(DisableBulletAfterTime(bulletPush, 4f));
                }
            }
        }
    }

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

    private IEnumerator DisableBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bullet.SetActive(false);
    }
}
