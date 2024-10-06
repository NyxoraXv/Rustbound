using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleAnimatorControl : MonoBehaviour
{

    [SerializeField] private int animType;
    private Animator animator;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("idleType", animType);
    }

    

}
