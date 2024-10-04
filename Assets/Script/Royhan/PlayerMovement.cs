using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootForce = 5f;
    [SerializeField] private int poolSize = 10; // Ukuran pool
    [SerializeField] private float sprintWalkPercentage = 50f;
    private float speed = 5f;
    private float rotationSpeed = 5f;
    private Transform cameraTransform;

    private Vector3 moveDirection;
    private Vector2 _movementInput;
    private Rigidbody _rigidbody;
    private VariableComponent variableComponent;

    private List<GameObject> bulletPool; // Pool untuk peluru
    private Camera mainCamera; // Untuk mengambil posisi mouse
    private Vector3 direction;
    private Animator animator;
    private int speedParam = Animator.StringToHash("Speed");
    private int fireParam = Animator.StringToHash("Fire");
    private bool onSprint = false;

    private void Awake()
    {
        
        _rigidbody = GetComponent<Rigidbody>();
        variableComponent = GetComponent<VariableComponent>();
        animator = GetComponent<Animator>();

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

        // Mengecek apakah player bergerak maju atau mundur
        float dotProduct = Vector3.Dot(moveDirection.normalized, direction.normalized);

        if (dotProduct > 0)
        {
            Debug.Log("maju");
            if (!onSprint)
                animator.SetFloat(speedParam, 0.5f);

            else
                animator.SetFloat(speedParam, 1f);

        }
        else if (dotProduct < 0)
        {
            Debug.Log("mundur");
            if (!onSprint)
                animator.SetFloat(speedParam, -0.5f);

            else
                animator.SetFloat(speedParam, -1f);
        }
        else
        {
            animator.SetFloat(speedParam, 0f);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("State"))
        // if (context.performed)
        {
            Debug.Log( "tag ");
            _movementInput = context.ReadValue<Vector2>();
            // Debug.Log("move " + _movementInput);
            // Debug.Log("dir " + direction);
            // Debug.Log("dis " + Vector2.Distance(new Vector2 (direction.x, direction.z).normalized, _movementInput));

            if (_movementInput != Vector2.zero && !_rigidbody.freezeRotation)
            {
                _rigidbody.freezeRotation = true;
            }

        }
        
        if (_movementInput.magnitude < 0.1f)
        {
            _movementInput = Vector2.zero;
        }
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        // if(context.)
        // Debug.Log("forward");
        // if(context.canceled)
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        // if (moveDirection != Vector3.zero)
        // {
        //     Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        // }

        // Menentukan arah tembakan berdasarkan posisi mouse
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hit point berdasarkan mouse tanpa mengubah posisi Y dari peluru
            Vector3 targetPosition = new Vector3(hit.point.x, shootPos.position.y, hit.point.z);

            targetPosition.z -= Mathf.Abs(hit.point.normalized.x);
            // Menghitung arah tembakan dengan mempertahankan posisi Y peluru
            direction = (targetPosition - shootPos.position).normalized;


            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 0.5f);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool(fireParam, true);
            GameObject bulletPush = GetPooledBullet();
            if (bulletPush != null)
            {

                // Instantiate(bulletPush, hit.point, quaternion.identity).SetActive(true);
                // print("dir" + direction);
                // print("target" + targetPosition);
                // print("hit" + hit.point.normalized);

                bulletPush.transform.position = shootPos.position;

                // Mengatur rotasi peluru agar tidak mengubah sumbu Y
                bulletPush.transform.rotation = Quaternion.LookRotation(direction);

                bulletPush.SetActive(true);
                bulletPush.GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset velocity sebelum menembak
                bulletPush.GetComponent<Rigidbody>().AddForce(direction * shootForce, ForceMode.Impulse);

                StartCoroutine(DisableBulletAfterTime(bulletPush, 4f));
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

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            speed = variableComponent.speed + (variableComponent.speed * sprintWalkPercentage/100);
            onSprint = true;
        }
        else if (context.canceled)
        {
            speed = variableComponent.speed;
            onSprint = false;
        }
    }
}
