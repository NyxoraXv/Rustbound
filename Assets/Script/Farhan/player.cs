using UnityEngine;

public class Player : MonoBehaviour
{
    // Variabel untuk EXP dan Level
    public int currentExp = 0;
    public int maxExp = 350;
    public int level = 1;
    public int availableStacks = 0; // Stack yang bisa digunakan untuk meningkatkan kemampuan

    // Variabel untuk atribut player
    public float vitality = 1.0f;   // Health points
    public float agility = 1.0f;    // Movement speed
    public float endurance = 1.0f;  // Max stamina for sprint
    public float luck = 1.0f;       // Meningkatkan jumlah gold yang diterima setelah round selesai

    public int baseGoldReward = 100; // Jumlah gold dasar setelah round selesai

    // Fungsi untuk menambahkan EXP
    public void AddExp(int exp)
    {
        currentExp += exp;
        Debug.Log("EXP added: " + exp);

        // Cek apakah EXP telah mencapai atau melewati maxExp
        if (currentExp >= maxExp)
        {
            LevelUp();
        }

        Debug.Log("Total EXP: " + currentExp);
    }

    // Fungsi untuk naik level
    private void LevelUp()
    {
        level++;
        currentExp -= maxExp; // Reset EXP atau sisa jika lebih dari maxExp
        availableStacks += 3; // Tambahkan stack setiap naik level

        Debug.Log("Level Up! Current Level: " + level);
        Debug.Log("Available Stacks: " + availableStacks);
    }

    // Fungsi untuk meningkatkan atribut menggunakan stack
    public void UpgradeAbility(string ability)
    {
        if (availableStacks > 0)
        {
            switch (ability)
            {
                case "Vitality":
                    IncreaseStat(ref vitality);  // Naikkan vitality
                    break;
                case "Agility":
                    IncreaseStat(ref agility);   // Naikkan agility (movement speed)
                    break;
                case "Endurance":
                    IncreaseStat(ref endurance); // Naikkan endurance (max stamina)
                    break;
                case "Luck":
                    IncreaseStat(ref luck);      // Naikkan luck (meningkatkan jumlah gold)
                    break;
                default:
                    Debug.LogWarning("Invalid ability: " + ability);
                    return;
            }

            availableStacks--;  // Kurangi stack setelah upgrade
            Debug.Log(ability + " upgraded. Remaining Stacks: " + availableStacks);
        }
        else
        {
            Debug.Log("No stacks available to upgrade.");
        }
    }

    // Fungsi untuk menaikkan stat dengan aturan 1.0 ke 1.1 hingga 1.9, lalu 2.0
    private void IncreaseStat(ref float stat)
    {
        if (stat < 2.0f)
        {
            stat = Mathf.Round((stat + 0.1f) * 10f) / 10f;  // Naikkan 0.1, lalu bulatkan menjadi 1 desimal
            Debug.Log("Stat increased to: " + stat);
        }
        else
        {
            Debug.Log("Stat has reached maximum limit.");
        }
    }

    // Fungsi untuk menghitung gold dengan bonus Luck (1% bonus setiap 0.1 Luck)
    public int CalculateGoldReward()
    {
        float luckBonusMultiplier = 1 + (luck - 1) * 0.01f;  // Setiap 0.1 Luck menambah 1% bonus
        int totalGold = Mathf.RoundToInt(baseGoldReward * luckBonusMultiplier);

        Debug.Log("Base Gold: " + baseGoldReward + ", Luck Multiplier: " + luckBonusMultiplier + ", Total Gold: " + totalGold);
        return totalGold;
    }

    // Fungsi untuk menyerang enemy
    // private void Attack(Enemy enemy)
    // {
    //     // Misalnya jika menyerang enemy dan menyebabkan musuh mati
    //     enemy.Die(this);  // Kirim referensi player ke fungsi Die
    // }
}
