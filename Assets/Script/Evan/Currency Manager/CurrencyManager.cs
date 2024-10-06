using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private int currentCurrency;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Method to add currency
    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        Debug.Log("Currency added. Current currency: " + currentCurrency);
    }

    // Method to spend currency
    public bool SpendCurrency(int amount)
    {
        if (currentCurrency >= amount)
        {
            currentCurrency -= amount;
            Debug.Log("Currency spent. Current currency: " + currentCurrency);
            return true;
        }
        else
        {
            Debug.Log("Not enough currency.");
            return false;
        }
    }

    // Method to check the current currency
    public int GetCurrency()
    {
        return currentCurrency;
    }
}
