using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Building : VariableComponent
{
    public int SoundToChoose = 9; // Variable to choose sound
    private SoundManager soundManager;

    public void Start()
    {
        _currentHealth = maxHealth;

        soundManager = FindAnyObjectByType<SoundManager>();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        soundManager.PlaySFX(SoundToChoose); 
        
        if (GetCurrentHealth() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
