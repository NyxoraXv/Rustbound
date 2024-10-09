using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : VariableComponent
{
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform rotateBody;
    // [SerializeField] private Transform body;
    // [SerializeField] private Transform leftFoot;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootForce = 5f;
    [SerializeField] private int poolSize = 10; // Ukuran pool
    [SerializeField] private float sprintWalkPercentage = 50f;
    private Transform cameraTransform;

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
    }

    private void FixedUpdate()
    {
        if (!shoot)
        {

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection = forward * _movementInput.y + right * _movementInput.x;
            _rigidbody.velocity = new Vector3(moveDirection.x * _speed, _rigidbody.velocity.y, moveDirection.z * _speed);

            // Mengecek apakah player bergerak maju atau mundur
            float dotProduct = Vector3.Dot(moveDirection.normalized, direction.normalized);

            if (dotProduct != 0)
            {
                Debug.Log("maju");
                animator.SetBool(walkParam, true);
                if (onSprint)
                    animator.SetBool(runParam, true);

                else
                    animator.SetBool(runParam, false);

            }
            // else if (dotProduct < 0)
            // {
            //     Debug.Log("mundur");
            //     if (!onSprint)
            //         animator.SetFloat(walkParam, -0.5f);

            //     else
            //         animator.SetFloat(walkParam, -1f);
            // }
            else
            {
                animator.SetBool(runParam, false);
                animator.SetBool(walkParam, false);
            }
        }
    }
    private void LateUpdate()
    {
        // if (!shoot)
        // {
        HandleRotation(moveDirection);
        // }
    }
    // public void Del () => Destroy(gameObject, 0.2f);
    protected override void Die ()
    {
        Debug.Log("player Die");
        // Destroy(_rigidbody);
        Destroy(this);
        // Destroy
        animator.SetTrigger(dieParam);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        // if (currentState.IsName("State"))
        // // if (context.performed)
        if (!shoot)
        {
            Debug.Log("tag ");
            _movementInput = context.ReadValue<Vector2>();
            // Debug.Log("move " + _movementInput);
            // Debug.Log("dir " + direction);
            // Debug.Log("dis " + Vector2.Distance(new Vector2 (direction.x, direction.z).normalized, _movementInput));

            if (_movementInput != Vector2.zero && !_rigidbody.freezeRotation)
            {
                _rigidbody.freezeRotation = true;
            }

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
        // Menentukan arah tembakan berdasarkan posisi mouse
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hit point berdasarkan mouse tanpa mengubah posisi Y dari peluru
            Vector3 targetPosition = new Vector3(hit.point.x, shootPos.position.y, hit.point.z);

            targetPosition.z -= Mathf.Abs(hit.point.normalized.x);

            // Menghitung arah tembakan dengan mempertahankan posisi Y peluru
            direction = (targetPosition - shootPos.position).normalized;

            // Hitung rotasi target yang diinginkan
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Dapatkan sudut Euler dari rotasi yang diinginkan
            Vector3 eulerRotation = toRotation.eulerAngles;

            // Biarkan rotasi Y tidak terbatas atau clamp ke rentang 0 hingga 360 jika diperlukan
            // eulerRotation.y *= 1.6f;


            // Terapkan rotasi baru yang sudah dibatasi
            rotateBody.rotation = Quaternion.Euler(eulerRotation);
            // Debug.Log(Quaternion.Euler(eulerRotation));

            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(moveDirection),
                    rotationSpeed * Time.deltaTime
                );
                // leftFoot.rotation = Quaternion.Slerp(
                //     leftFoot.rotation, 
                //     Quaternion.LookRotation(moveDirection), 
                //     rotationSpeed * Time.deltaTime
                // );
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
