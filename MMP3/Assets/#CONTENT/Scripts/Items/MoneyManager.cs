using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [Header("Money")]
    public int totalMoney = 0;
    
    [Header("UI")]
    public TextMeshProUGUI moneyText;
    public string currencySymbol = "$";
    
    [Header("Effects")]
    public bool animateMoneyChange = true;
    public float animationDuration = 0.5f;
    
    // Private variables for animation
    private int displayedMoney = 0;
    private int targetMoney = 0;
    private float animationStartTime = 0f;
    private bool isAnimating = false;
    
    private void Awake()
    {
        // Simple singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Initialize values
        displayedMoney = totalMoney;
        targetMoney = totalMoney;
        
        UpdateMoneyDisplay();
    }
    
    private void Update()
    {
        // Animate money change if needed
        if (isAnimating)
        {
            float elapsed = Time.time - animationStartTime;
            float progress = Mathf.Clamp01(elapsed / animationDuration);
            
            if (progress >= 1.0f)
            {
                // Animation complete
                displayedMoney = targetMoney;
                isAnimating = false;
            }
            else
            {
                // Interpolate for smooth counting effect
                displayedMoney = Mathf.RoundToInt(Mathf.Lerp(displayedMoney, targetMoney, progress));
            }
            
            UpdateMoneyText();
        }
    }
    
    public void AddMoney(int amount)
    {
        // Update the actual total immediately
        totalMoney += amount;
        
        if (animateMoneyChange && amount > 0)
        {
            // Setup for animated counting
            targetMoney = totalMoney;
            animationStartTime = Time.time;
            isAnimating = true;
        }
        else
        {
            // No animation, update immediately
            displayedMoney = totalMoney;
        }
        
        UpdateMoneyDisplay();
        Debug.Log("Added " + currencySymbol + amount + ". Total: " + currencySymbol + totalMoney);
    }
    
    public bool SpendMoney(int amount)
    {
        if (totalMoney >= amount)
        {
            totalMoney -= amount;
            
            if (animateMoneyChange)
            {
                // Setup for animated counting down
                targetMoney = totalMoney;
                animationStartTime = Time.time;
                isAnimating = true;
            }
            else
            {
                // No animation, update immediately
                displayedMoney = totalMoney;
            }
            
            UpdateMoneyDisplay();
            return true;
        }
        return false;
    }
    
    private void UpdateMoneyDisplay()
    {
        // Update the actual displayed UI text
        if (moneyText != null)
        {
            if (!animateMoneyChange)
            {
                // If not animating, just show the total
                moneyText.text = currencySymbol + totalMoney.ToString();
            }
            else
            {
                // Otherwise update just the displayed value
                UpdateMoneyText();
            }
        }
    }
    
    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = currencySymbol + displayedMoney.ToString();
        }
    }
    
    // Helper to get the current displayed money (useful for UI)
    public int GetDisplayedMoney()
    {
        return displayedMoney;
    }
    
    // Helper to get the actual total money
    public int GetTotalMoney()
    {
        return totalMoney;
    }
}
