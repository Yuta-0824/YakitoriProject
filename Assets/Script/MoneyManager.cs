using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public TextMeshProUGUI moneyText;
    private int currentMoney = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // --- 修正ポイント：セーブデータの読み込みをやめる ---
        // currentMoney = PlayerPrefs.GetInt("TotalMoney", 0); // これを削除
        currentMoney = 0; // 常に0円からスタート
    }

    void Start()
    {
        UpdateMoneyUI();
    }

    // お金を増やす
    public void AddMoney(int amount)
    {
        currentMoney += amount;

        // PlayerPrefs.SetInt("TotalMoney", currentMoney); // 保存もしない
        // PlayerPrefs.Save(); // 削除

        UpdateMoneyUI();
    }

    // お金を減らす（買い物）
    public bool UseMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"所持金: {currentMoney}円";
        }
    }

    // もしリトライ時に明示的にリセットしたい場合に呼ぶ関数
    public void ResetMoney()
    {
        currentMoney = 0;
        UpdateMoneyUI();
    }
}