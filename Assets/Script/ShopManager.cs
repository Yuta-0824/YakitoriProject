using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("ショップ画面の親オブジェクト")]
    public GameObject shopPanel;

    void Start()
    {
        // ゲーム開始時はショップを閉じておく
        shopPanel.SetActive(false);
    }

    // アップグレードボタン（開く）から呼ぶ関数
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f; // ゲームを一時停止
        Debug.Log("ショップを開きました");
    }

    // ×ボタン（閉じる）から呼ぶ関数
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f; // ゲームを再開
        Debug.Log("ショップを閉じました");
    }
}