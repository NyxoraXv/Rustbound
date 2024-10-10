using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [SerializeField] private TextMeshProUGUI hp, sp, current_ammo, max_ammo, level;
    [SerializeField] private Image first_slot, second_slot, healthImage;
    [SerializeField] private PlayerMovement playerMovement;
    
    private void Awake() 
    {
        if (playerMovement == null)
        {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }    
    }

    private void Update() 
    {
        hp.text = $"{playerMovement._currentHealth}/{playerMovement.maxHealth}";
        healthImage.fillAmount = playerMovement._currentHealth/playerMovement.maxHealth;
    }
    
}
