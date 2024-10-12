using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerMovement : VariableComponent
{
    [Header("Player Died")]
    public GameObject playerDied;

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
    [SerializeField] private float staminaDecreaseRate = 10f;  // Pengurangan stamina per detik saat berlari
    [SerializeField] private float staminaRecoveryRate = 5f;   // Pemulihan stamina per detik saat tidak berlari
    public float maxStamina = 100f;  // Maksimum stamina
    public GameObject bulletPrefab;
    [HideInInspector] public float bulletDamage;
    private static int[] weaponIndex = {0, 0};
    private Transform cameraTransform;
    private static List<GameObject> weaponCollection = new List<GameObject>();
    private Vector3 moveDirection;
    private Vector2 _movementInput;
    private Rigidbody _rigidbody;
    private Vector2 _scrollWeapon;
    private bool onBuilding;
    private bool onMenu;

    private List<GameObject> bulletPool; // Pool untuk peluru
    private Camera mainCamera; // Untuk mengambil posisi mouse
    private float _speed;
    private int dieParam = Animator.StringToHash("Die");
    private Vector3 direction;
    private Animator animator;
    private int walkParam = Animator.StringToHash("IsWalk");
    private int runParam = Animator.StringToHash("IsRun");
    private int fireParam = Animator.StringToHash("Fire");
    private static int indexWeapon = 0;
    private WeaponHandler activeWeaponHandler;
    private bool isFiring = false;

    private int weaponSlot = 1; // Only 1 and 2

    public float currentStamina {get; private set;}  // Stamina saat ini
    
    private bool isSprinting = false; // Status apakah sedang berlari
    private float rateFire;
    private float timeNow = 0;

    // private static Dictionary<string, int> weaponIndexes = new Dictionary<string, int>();

    public void EquipWeapon(WeaponHandler newWeaponHandler)
    {
        activeWeaponHandler = newWeaponHandler;
        rateFire = activeWeaponHandler.weaponRateOfFire;

        // Set position, parent, etc., of the weapon based on the player's weapon holding position (e.g., hands).
        activeWeaponHandler.transform.SetParent(weaponGrab); // Attach to player's hand or weapon holding position
        activeWeaponHandler.transform.localPosition = Vector3.zero; // Adjust for correct positioning
    }

    private void initiator()
    {
        CurrencyManager.Instance.AddCurrency(2000);
        WeaponManager.Instance.BuyWeapon(0); // Purchase the first weapon (or select the default one)
        WeaponManager.Instance.EquipWeapon(0, 1); // Equip the first weapon in slot 1
        WeaponManager.Instance.EquipWeapon(0, 2);
        TurretManager.Instance.buyTurret(0);
        SwitchWeapon(1); // Switch to weapon in slot 1
    }

    private void Awake()
    {

        //CurrencyManager.Instance.AddCurrency(1200);
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _currentHealth = maxHealth;
        
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        mainCamera = Camera.main;

        _speed = speed;

        currentStamina = maxStamina;
        initiator();
    }

    private void Update()
    {
        if (onBuilding || onMenu)
            return;
        if (UIController.instance.currentState != 0)
            return;

        HandleStamina();
        HandleFiring();
        HandleWeaponScroll();

        // if (timeNow > rateFire)
        //     timeNow = 0;
    }

    public void OnFire(InputAction.CallbackContext context)

    {
        if (context.performed)
        {
            isFiring = true; // Start firing
        }
        else if (context.canceled)
        {
            timeNow = 0;
            isFiring = false; // Stop firing
        }
    }

    private void HandleFiring()
    {
        timeNow += Time.deltaTime;
        if (isFiring && activeWeaponHandler != null && timeNow > rateFire)
        {
            timeNow = 0;
            activeWeaponHandler.Fire(); // Call the Fire method on the active weapon
        }
    }

    private void FixedUpdate()
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
            UpdateMovementAnimations(moveDirection, direction, isSprinting);
    }


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
        CanvasGroup canvasGroup = playerDied.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = playerDied.AddComponent<CanvasGroup>(); // Add CanvasGroup if not present
            }

            playerDied.SetActive(true);  // Step 1: Set active
            
            canvasGroup.alpha = 0f;
        
            canvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                // Step 3: Optional delay after fading in
                DOVirtual.DelayedCall(3f, () =>
                {
                    // Step 4: Fade out the roundFinish
                    canvasGroup.DOFade(0f, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        // Step 5: Set inactive after fade out
                        playerDied.SetActive(false);
                        
                    });
                });
            });

        // Destroy(_rigidbody);
        Destroy(this);
        // Destroy
        animator.SetBool(dieParam, true);
    }

    public void OnMove(InputAction.CallbackContext context)
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
        if (UIController.instance.currentState != 0)
            return;

        // Raycast to determine aiming direction based on mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerRaycast))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, shootPos.position.y, hit.point.z);
            direction = (targetPosition - shootPos.position).normalized;

            // Calculate desired upper body rotation (only Y-axis)
            Quaternion upperBodyRotation = Quaternion.LookRotation(direction, Vector3.up);
            upperBodyRotation = Quaternion.Euler(0, upperBodyRotation.eulerAngles.y + 60.378f, 0);  // Adjust for offset

            // Apply the upper body rotation with smoothing
            rotateBody.rotation = Quaternion.Slerp(rotateBody.rotation, upperBodyRotation, rotationSpeed * Time.deltaTime);

            // Handle lower body rotation (only Y-axis)
            if (moveDirection != Vector3.zero)
            {
                // Determine the desired forward direction based on movement
                Quaternion moveRotation = Quaternion.LookRotation(moveDirection);
                float dotProduct = Vector3.Dot(moveDirection.normalized, direction.normalized);

                if (dotProduct < 0) // Moving backward
                {
                    // Match lower body to upper body's Y rotation
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.Euler(0, rotateBody.rotation.eulerAngles.y, 0), // Match lower body to upper body's Y rotation
                        rotationSpeed * Time.deltaTime
                    );
                }
                else
                {
                    // Normal forward movement rotation (only Y-axis)
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.Euler(0, moveRotation.eulerAngles.y, 0), // Only Y-axis
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
        }
    }


    private void HandleWeaponScroll()
    {
        // Check for mouse scroll input
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        // Set weaponSlot based on scroll direction
        if (scrollValue > 0)
        {
            weaponSlot = 1; // Scroll up sets weapon slot to 1
        }
        else if (scrollValue < 0)
        {
            weaponSlot = 2; // Scroll down sets weapon slot to 2
        }

        // Only call SwitchWeapon if the weapon slot has changed
        if (weaponSlot != previousWeaponSlot)
        {
            Debug.Log("Weapon Slot: " + weaponSlot);
            SwitchWeapon(weaponSlot);
            previousWeaponSlot = weaponSlot; // Update the previous weapon slot
        }
    }

    private void SwitchWeapon(int slot)
    {
        // Implement your weapon switching logic here
        // For example, using the WeaponDatabase to get the active weapon based on the slot
        WeaponHandler newActiveWeapon = null; // To store the new active weapon for updating UI

        foreach (WeaponHandler wh in WeaponManager.Instance.weaponCache.GetComponentsInChildren<WeaponHandler>())
        {
            if (wh.currentSlot == slot)
            {
                if (activeWeaponHandler != null)
                {
                    HUDController.instance.SwapWeaponImagesSlot2(WeaponManager.Instance.weaponDatabase.GetWeaponByID(activeWeaponHandler.ID).weaponImage);
                    activeWeaponHandler.transform.localPosition = new Vector3(999, 999);
                    activeWeaponHandler.transform.parent = WeaponManager.Instance.weaponCache.transform;
                }
                wh.transform.parent = weaponGrab;
                wh.transform.localPosition = Vector3.zero;
                wh.transform.localRotation = Quaternion.identity;

                newActiveWeapon = wh; // Store the new active weapon
                activeWeaponHandler = wh;
                rateFire = activeWeaponHandler.weaponRateOfFire;
                HUDController.instance.SwapWeaponImagesSlot1(WeaponManager.Instance.weaponDatabase.GetWeaponByID(wh.ID).weaponImage);
            }
        }

        // Update the inventory UI to reflect the current weapon slot
        if (newActiveWeapon != null)
        {
            // Assuming you have a reference to your InventoryUIAnimator component
            InventoryUIAnimator inventoryUIAnimator = FindObjectOfType<InventoryUIAnimator>();
            if (inventoryUIAnimator != null)
            {
                Sprite primarySprite = WeaponManager.Instance.weaponDatabase.GetWeaponByID(activeWeaponHandler.ID).weaponImage; // Update with your primary weapon
                Sprite secondarySprite = WeaponManager.Instance.weaponDatabase.GetWeaponByID(1).weaponImage; // Assuming index 1 is your secondary weapon
                inventoryUIAnimator.UpdateWeaponIcons(primarySprite, secondarySprite);
            }
        }
    }


    private int previousWeaponSlot = 1;

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && currentStamina > 0)
        {
            _speed = speed + (speed * sprintWalkPercentage / 100);
            isSprinting = true;
        }
        else if (context.canceled || currentStamina <= 0)
        {
            _speed = speed;
            isSprinting = false;
        }
    }

    private void HandleStamina()
    {
        if (currentStamina <= 0)
        {
            _speed = speed;
            isSprinting = false;
        }
        
        if (isSprinting && currentStamina > 0)
        {
            // Kurangi stamina saat berlari
            currentStamina -= staminaDecreaseRate * Time.deltaTime;
            // Debug.Log("berkurang...");
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Pastikan stamina tidak di bawah 0
        }
        else
        {
            // Pulihkan stamina saat tidak berlari
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Pastikan stamina tidak melebihi maksimum
        }


        // Opsional: Debug log untuk melihat nilai stamina
        // Debug.Log("Stamina: " + currentStamina);
    }
}
