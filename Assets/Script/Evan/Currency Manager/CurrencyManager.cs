using UnityEngine;
using TMPro; // For TextMeshPro
using DG.Tweening; // For DoTween

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public TextMeshProUGUI currencyText; // Reference to the TextMeshPro UI element
    private int currentCurrency;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        UpdateCurrencyUI(); // Initialize the UI with the current currency value
    }

    // Method to add currency
    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        Debug.Log("Currency added. Current currency: " + currentCurrency);

        // Update the UI
        UpdateCurrencyUI();

        // Animate the currency text by scaling it up and back down
        AnimateCurrencyText();
    }

    // Method to spend currency
    public bool SpendCurrency(int amount)
    {
        if (HasEnoughCurrency(amount))
        {
            currentCurrency -= amount;
            Debug.Log("Currency spent. Current currency: " + currentCurrency);

            // Update the UI
            UpdateCurrencyUI();

            return true;
        }
        else
        {
            Debug.Log("Not enough currency.");
            return false;
        }
    }

    // Method to check if there is enough currency
    public bool HasEnoughCurrency(int amount)
    {
        return currentCurrency >= amount;
    }

    // Method to check the current currency
    public int GetCurrency()
    {
        return currentCurrency;
    }

    // Update the currency display
    private void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = currentCurrency.ToString();
        }
    }

    // Animate the currency text with a scaling effect
    private void AnimateCurrencyText()
    {
        if (currencyText != null)
        {
            // Reset the scale first
            currencyText.rectTransform.localScale = Vector3.one;

            // Animate scale to 1.2 and back to 1 over 0.5 seconds
            currencyText.rectTransform.DOScale(1.2f, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                currencyText.rectTransform.DOScale(1f, 0.25f).SetEase(Ease.OutQuad);
            });
        }
    }
}
