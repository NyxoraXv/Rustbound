using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : VariableComponent
{
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform weaponGrab;
    [SerializeField] private WeaponDatabase weapons;
    [SerializeField] private Transform rotateBody;
    // [SerializeField] private Transform body;
    // [SerializeField] private Transform leftFoot;
    [SerializeField] private float shootForce = 5f;
    [SerializeField] private int poolSize = 10; // Ukuran pool
    [SerializeField] private float sprintWalkPercentage = 50f;
    [SerializeField] private LayerMask layerRaycast;
    public GameObject bulletPrefab;
    [HideInInspector] public float bulletDamage;
    private static int[] weaponIndex = {0, 0};
    private Transform cameraTransform;
    private static List<GameObject> weaponCollection = new List<GameObject>();
    private Vector3 moveDirection;
    private Vector2 _movementInput;
    private Rigidbody _rigidbody;

    private List<GameObject> bulletPool; // Pool untuk peluru
    private Camera mainCamera; // Untuk mengambil posisi mouse
    private float _speed;
    private int dieParam = Animator.StringToHash("Die");
    private Vector3 direction;
    private Animator animator;
    private int walkParam = Animator.StringToHash("IsWalk");
    private int runParam = Animator.StringToHash("IsRun");
    private int fireParam = Animator.StringToHash("Fire");
    private bool onSprint = false;
    private bool shoot = false;
    private static int indexWeapon = 0;
    // private static Dictionary<string, int> weaponIndexes = new Dictionary<string, int>();

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _currentHealth = maxHealth;
        
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        mainCamera = Camera.main;

        _speed = speed;

        // Inisialisasi pooling
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            bulletPool.Add(obj);
        }

        
        foreach (GameObject obj in weapons.GetAllWeaponObject())
        {
            GameObject objec = Instantiate(obj, weaponGrab);
            objec.GetComponent<WeaponComponent>().playerMovement = this;
            objec.SetActive(false);
            // weaponIndexes.Add(objec.name, weaponCollection.Count);
            weaponCollection.Add(objec);
        }
        weaponCollection[indexWeapon].SetActive(true);
        Bullet.bulletDamage = weapons.weapons[indexWeapon].weaponDamage;
        Bullet.rangeAttack = weapons.weapons[indexWeapon].weaponAccuracy;
    }


    private void FixedUpdate()
    {
        if (!shoot)
        {
            // Get forward and right directions relative to the camera
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Flatten the Y axis (ignore vertical movement)
            forward.y = 0f;
            right.y = 0f;

            // Normalize directions
            forward.Normalize();
            right.Normalize();

            // Calculate move direction based on input
            moveDirection = forward * _movementInput.y + right * _movementInput.x;
            _rigidbody.velocity = new Vector3(moveDirection.x * _speed, _rigidbody.velocity.y, moveDirection.z * _speed);

            // Check if the player is moving (dot product logic)
            float dotProduct = Vector3.Dot(moveDirection.normalized, direction.normalized);

            // Call the movement animation update function
            UpdateMovementAnimations(moveDirection, direction, onSprint);

            // Weapon selection based on key inputs
            if (Input.GetKeyDown(KeyCode.X))
            {
                SelectWeapon(1);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                SelectWeapon(2);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                SelectWeapon(3);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                SelectWeapon(4);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Array.Reverse(weaponIndex);
                SelectWeapon(weaponIndex[0]);
            }
        }
    }


    // Function to update movement animations
    // Function to update movement animations
    void UpdateMovementAnimations(Vector3 moveDirection, Vector3 direction, bool onSprint)
    {
        // Calculate the dot product between movement direction and facing direction
        float dotProduct = Vector3.Dot(moveDirection.normalized, direction.normalized);

        if (dotProduct > 0)
        {
            // Moving forward
            animator.SetBool("IsWalk", true);       // Walking forward
            animator.SetBool("isBackward", false);  // Not moving backward
        }
        else if (dotProduct < 0)
        {
            // Moving backward
            animator.SetBool("IsWalk", true);       // Walking backward
            animator.SetBool("isBackward", true);   // Set backward movement
        }
        else
        {
            // Stationary (not moving)
            animator.SetBool("IsWalk", false);      // No walking
            animator.SetBool("isBackward", false);  // No backward movement
        }

        // Apply sprinting logic regardless of direction
        if (onSprint)
        {
            animator.SetBool("IsRun", true);  // Set sprinting animation
        }
        else
        {
            animator.SetBool("IsRun", false); // Turn off sprinting
        }
    }


    public void SetWeapon(int index, bool isPrimary)
    {
        if (isPrimary)
        {
            weaponIndex[0] = index;
            SelectWeapon(index);
        }
        else
        {
            weaponIndex[1] = index;
        }
        
    }

    private  void SelectWeapon(int index)
    {

        weaponCollection[indexWeapon].SetActive(false);
        indexWeapon = index;
        weaponCollection[indexWeapon].SetActive(true);
        Bullet.bulletDamage = weapons.weapons[indexWeapon].weaponDamage;
        Bullet.rangeAttack = weapons.weapons[indexWeapon].weaponAccuracy;

    }

    private void LateUpdate()
    {
        if (Time.timeScale != 0)
        {
            HandleRotation(moveDirection);
        }
    }
    // public void Del () => Destroy(gameObject, 0.2f);
    protected override void Die ()
    {
        Debug.Log("player Die");
        // Destroy(_rigidbody);
        Destroy(this);
        // Destroy
        animator.SetBool(dieParam, true);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        // if (currentState.IsName("State"))
        // // if (context.performed)
        if (!shoot)
        {
            // Debug.Log("tag ");
            _movementInput = context.ReadValue<Vector2>();
            // Debug.Log("move " + _movementInput);
            // Debug.Log("dir " + direction);
            // Debug.Log("dis " + Vector2.Distance(new Vector2 (direction.x, direction.z).normalized, _movementInput));

            // if (_movementInput != Vector2.zero && !_rigidbody.freezeRotation)
            // {
            //     _rigidbody.freezeRotation = true;
            // }

        }
        else
        {
            _movementInput = Vector2.zero;
        }

        if (_movementInput.magnitude < 1f)
        {
            _movementInput = Vector2.zero;
        }
        Debug.Log(_movementInput);

        // if (context.canceled)
        // {
        //     _rigidbody.velocity = Vector2.zero;
        // }
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        // if(context.)
        // Debug.Log("forward");
        // if(context.canceled)
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        // Raycast to determine aiming direction based on mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerRaycast))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, shootPos.position.y, hit.point.z);
            targetPosition.z -= Mathf.Abs(hit.point.normalized.x);
            direction = (targetPosition - shootPos.position).normalized;

            // Calculate desired upper body rotation
            Quaternion upperBodyRotation = Quaternion.LookRotation(direction, Vector3.up);
            Vector3 eulerRotation = upperBodyRotation.eulerAngles;
            eulerRotation.y += 60.378f;  // Adjust for offset

            // Apply the upper body rotation
            rotateBody.rotation = Quaternion.Euler(eulerRotation);

            // Handle lower body rotation
            if (moveDirection != Vector3.zero)
            {
                // If moving backward, align lower body with upper body's rotation
                float dotProduct = Vector3.Dot(moveDirection.normalized, direction.normalized);
                if (dotProduct < 0)  // Moving backward
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        rotateBody.rotation, // Match lower body to upper body's rotation
                        rotationSpeed * Time.deltaTime
                    );
                }
                else
                {
                    // Normal forward movement rotation
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(moveDirection),
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
        }
    }


    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shoot = true;
            // Invoke("DeactiveShoot", 0.35f);
            // DeactiveShoot();
            animator.SetTrigger(fireParam);

        }
    }
    public void Shoot()
    {
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

    public void DeactiveShoot() => shoot = false;

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
        if (context.performed)
        {
            _speed = speed + (speed * sprintWalkPercentage / 100);
            onSprint = true;
        }
        else if (context.canceled)
        {
            _speed = speed;
            onSprint = false;
        }
    }
}
