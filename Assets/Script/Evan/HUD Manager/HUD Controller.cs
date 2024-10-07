using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [SerializeField] private TextMeshProUGUI hp, sp, round, zombie, current_ammo, max_ammo, level;
    [SerializeField] private Image first_slot, second_slot;

    private void init(int maxHP, int maxSP)
    {
        
    }
}
