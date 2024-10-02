using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableComponent : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float maxHealth;
    private float _currentHealth;

    private void Start() 
    {
        _currentHealth = maxHealth;    
    }
}
