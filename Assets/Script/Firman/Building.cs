using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Building : VariableComponent, ITurret
{
    [Header("Building ID")]
    public int buildingID;

    [Header("SFX Destroy")]
    public int soundDestroy = 9; // Variable to choose sound
    
    [Header("SFX Take Damage")]
    public int soundTakeDamage = 9; // Variable to choose sound

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

        soundManager.PlaySFX(soundTakeDamage); 

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
        PlacementSystem.Instance.RemoveObject(buildingID);
        soundManager.PlaySFX(soundDestroy);
        Destroy(gameObject); // Destroy the turret GameObject
    }

    public void SetIsPreviewObject(bool isPreview)
    {
        isPreviewObject = isPreview; // Method to set the preview object flag
    }
}
