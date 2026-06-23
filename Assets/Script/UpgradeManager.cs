using UnityEngine;
using UnityEngine.UI; // ボタンの制御に必要

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("アップグレード状態")]
    public bool isAutoCookOn = false;
    public bool isAutoCollectOn = false;

    [Header("コスト設定")]
    public int autoCookCost = 500;
    public int autoCollectCost = 1000;

    [Header("UIボタン参照")]
    public Button autoCookButton;
    public Button autoCollectButton;

    void Awake()
    {
        // シングルトンの設定
        if (Instance == null) Instance = this;

        // --- 修正ポイント：保存データを読み込まず、必ず false で開始する ---
        ResetUpgrades();
    }

    // アップグレードを初期状態に戻す
    public void ResetUpgrades()
    {
        isAutoCookOn = false;
        isAutoCollectOn = false;

        // ボタンを再び押せるようにする
        if (autoCookButton != null) autoCookButton.interactable = true;
        if (autoCollectButton != null) autoCollectButton.interactable = true;

        Debug.Log("アップグレードをリセットしました");
    }

    public void BuyAutoCook()
    {
        if (isAutoCookOn) return;

        if (MoneyManager.Instance != null && MoneyManager.Instance.UseMoney(autoCookCost))
        {
            isAutoCookOn = true;
            if (autoCookButton != null) autoCookButton.interactable = false; // そのプレイ中だけ売り切れ
            Debug.Log("自動焼き器を購入！(このプレイ中のみ有効)");
            // PlayerPrefs.SetInt... の行は削除
        }
    }

    public void BuyAutoCollect()
    {
        if (isAutoCollectOn) return;

        if (MoneyManager.Instance != null && MoneyManager.Instance.UseMoney(autoCollectCost))
        {
            isAutoCollectOn = true;
            if (autoCollectButton != null) autoCollectButton.interactable = false; // そのプレイ中だけ売り切れ
            Debug.Log("自動回収機を購入！(このプレイ中のみ有効)");
            // PlayerPrefs.SetInt... の行は削除
        }
    }
}