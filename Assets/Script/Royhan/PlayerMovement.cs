using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    private Transform cameraTransform; 
    
    private Vector3 moveDirection;
    private Vector2 _movementInput;
    private Rigidbody _rigidbody;
    private void Awake() 
    {
        _rigidbody = GetComponent<Rigidbody>();
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
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
        _rigidbody.velocity = new Vector3(moveDirection.x * _speed, _rigidbody.velocity.y, moveDirection.z * _speed);
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
}
