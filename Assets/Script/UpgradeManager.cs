using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    // アップグレード状態（これも保存する）
    public bool isAutoCookOn = false;
    public int autoCookCost = 0;
    public bool isAutoCollectOn = false; // 自動回収フラグ
    public int autoCollectCost = 0;   // 自動回収の価格（高めに設定）

    void Awake()
    {
        Instance = this;
        // 以前買ったか確認
        isAutoCookOn = (PlayerPrefs.GetInt("Upgrade_AutoCook", 0) == 1);
        isAutoCollectOn = (PlayerPrefs.GetInt("Upgrade_AutoCollect", 0) == 1);
    }

    // ボタンから呼ぶ購入関数
    public void BuyAutoCook()
    {
        if (isAutoCookOn) return; // 既に持っている

        if (MoneyManager.Instance.UseMoney(autoCookCost))
        {
            isAutoCookOn = true;
            PlayerPrefs.SetInt("Upgrade_AutoCook", 1);
            PlayerPrefs.Save();
            Debug.Log("自動焼き器を購入しました！");
            autoCookButton.interactable = false; // ボタンをグレーアウト
        }
    }
    public void BuyAutoCollect()
    {
        if (isAutoCollectOn) return;

        if (MoneyManager.Instance.UseMoney(autoCollectCost))
        {
            isAutoCollectOn = true;
            PlayerPrefs.SetInt("Upgrade_AutoCollect", 1);
            PlayerPrefs.Save();
            Debug.Log("自動回収機を購入しました！");
        }
    }
    public Button autoCookButton; // インスペクターでボタンを登録
}
