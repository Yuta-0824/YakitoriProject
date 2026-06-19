using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public TextMeshProUGUI moneyText;
    private int currentMoney = 0;

    void Awake()
    {
        Instance = this;
        // •Û‘¶‚³‚ê‚Ä‚¢‚é‚¨‹à‚ğ“Ç‚İ‚Ş
        currentMoney = PlayerPrefs.GetInt("TotalMoney", 0);
    }

    void Start()
    {
        UpdateMoneyUI();
    }

    // ‚¨‹à‚ğ‘‚â‚·i•ñVj
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        PlayerPrefs.SetInt("TotalMoney", currentMoney);
        PlayerPrefs.Save();
        UpdateMoneyUI();
    }

    // ‚¨‹à‚ğŒ¸‚ç‚·i”ƒ‚¢•¨j
    public bool UseMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            PlayerPrefs.SetInt("TotalMoney", currentMoney);
            PlayerPrefs.Save();
            UpdateMoneyUI();
            return true; // w“ü¬Œ÷
        }
        return false; // ‚¨‹à‚ª‘«‚è‚È‚¢
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null) moneyText.text = $"Š‹à: {currentMoney}‰~";
    }
}