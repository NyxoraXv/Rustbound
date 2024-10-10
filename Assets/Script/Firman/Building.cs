using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Building : VariableComponent, ITurret
{
    [Header("SFX")]
    public int SoundToChoose = 9; // Variable to choose sound

    [Header("VFX")]
    public ParticleSystem destroyVFX;
    private SoundManager soundManager;
    private bool isPreviewObject;

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
            DestroyBuilding();
        }
    }

    private void DestroyBuilding()
    {
        if (destroyVFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(destroyVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance.gameObject, 1f);
        }
        soundManager.PlaySFX(5);
        Destroy(gameObject); // Destroy the turret GameObject
    }

    public void SetIsPreviewObject(bool isPreview)
    {
        isPreviewObject = isPreview; // Method to set the preview object flag
    }
}
