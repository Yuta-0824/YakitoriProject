using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class YakitoriManager : MonoBehaviour
{
    [Header("焼き鳥の設定")]
    public GameObject yakitoriPrefab;
    public Transform[] spawnPoints;

    [Header("UI参照")]
    public TextMeshProUGUI timerText;

    [Header("ゲーム設定")]
    public float timeRemaining = 60f;
    public float inputCooldown = 0.3f;

    // --- 修正ポイント：List を 固定配列(Array) に変更 ---
    // これにより [0][1][2] という「場所」を固定できます
    public GameObject[] activeYakitoris;
    private int selectedIndex = 0;
    private float lastInputTime = 0f;
    private bool isGameActive = true;

    void Start()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.ResetUpgrades();
        }
        // スロットの数（Posの数）だけ席を用意する
        activeYakitoris = new GameObject[spawnPoints.Length];
    }

    void Update()
    {
        if (Time.timeScale == 0) return; // 一時停止中は動かさない
        if (!isGameActive) return;

        // タイマー処理
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timerText != null) timerText.text = $"TIME: {timeRemaining:F0}";
        }
        else { FinishGame(); }

        if (Time.time - lastInputTime < inputCooldown) return;

        HandleInputs();

        // --- 追加：自動回収のチェック ---
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.isAutoCollectOn)
        {
            CheckAutoCollect();
        }
        void CheckAutoCollect()
        {
            // 配置されている全スロットを確認
            for (int i = 0; i < activeYakitoris.Length; i++)
            {
                if (activeYakitoris[i] == null) continue;

                CookingCore core = activeYakitoris[i].GetComponent<CookingCore>();

                // 【条件】表も裏も100%を超えたら
                if (core.omoteProgress >= 100f && core.uraProgress >= 100f)
                {
                    Debug.Log($"<color=cyan>スロット {i} を自動回収します！</color>");

                    // そのスロットを回収する
                    AutoCollectAt(i);
                }
            }
        }
    }

    // 指定した番号の焼き鳥を回収する専用の関数
    void AutoCollectAt(int index)
    {
        GameObject target = activeYakitoris[index];
        CookingCore core = target.GetComponent<CookingCore>();

        // スコアと報酬の加算
        int points = CalculatePoints(core.omoteProgress, core.uraProgress);
        ScoreManager.Instance.AddScore(points);

        // 100円報酬（最高判定なら）
        if (core.omoteProgress >= 80f && core.uraProgress >= 80f && core.omoteProgress < 150f && core.uraProgress < 150f)
        {
            MoneyManager.Instance.AddMoney(100);
        }

        Judge(core.omoteProgress, core.uraProgress);

        // 削除
        Destroy(target);
        activeYakitoris[index] = null;

        RefreshSelection();
    }

    void HandleInputs()
    {
        // 選択切り替え
        if (GetLeftInput()) { ChangeSelection(-1); lastInputTime = Time.time; }
        if (GetRightInput()) { ChangeSelection(1); lastInputTime = Time.time; }

        // 増やす
        if (GetUpInput()) { SpawnYakitori(); lastInputTime = Time.time; }

        // 回収
        if (GetDownInput()) { CollectYakitori(); lastInputTime = Time.time; }
    }

    void SpawnYakitori()
    {
        // --- 修正ポイント：左から順に「空いている席」を探す ---
        int emptyIndex = -1;
        for (int i = 0; i < activeYakitoris.Length; i++)
        {
            if (activeYakitoris[i] == null) // もしここが空なら
            {
                emptyIndex = i;
                break; // 最初に見つけた空き場所で決定
            }
        }

        if (emptyIndex == -1)
        {
            Debug.LogWarning("すべての焼き場が埋まっています！");
            return;
        }

        // 見つけた空き場所（emptyIndex）に生成
        GameObject newYaki = Instantiate(yakitoriPrefab, spawnPoints[emptyIndex].position, spawnPoints[emptyIndex].rotation);
        activeYakitoris[emptyIndex] = newYaki;

        // 生成したものを今の操作対象にする
        selectedIndex = emptyIndex;

        // UIの紐付け
        CookingCore newCore = newYaki.GetComponent<CookingCore>();
        YakitoriUI ui = newYaki.GetComponentInChildren<YakitoriUI>();
        if (ui != null) ui.core = newCore;

        RefreshSelection();
    }

    void CollectYakitori()
    {
        // 今選んでいるスロットが空なら何もしない
        if (activeYakitoris[selectedIndex] == null) return;

        GameObject target = activeYakitoris[selectedIndex];
        CookingCore core = target.GetComponent<CookingCore>();
        if (core != null)
        {
            ScoreManager.Instance.AddScore(CalculatePoints(core.omoteProgress, core.uraProgress));
            Judge(core.omoteProgress, core.uraProgress);

            // --- 修正ポイント：最高判定なら100円加算 ---
            if (core.omoteProgress >= 80f && core.uraProgress >= 80f && core.omoteProgress < 150f && core.uraProgress < 150f)
            {
                MoneyManager.Instance.AddMoney(100);
                Debug.Log("<color=yellow>報酬100円ゲット！</color>");
            }

        }

        Destroy(target);
        activeYakitoris[selectedIndex] = null; // スロットを空にする

        RefreshSelection();
    }

    void ChangeSelection(int direction)
    {
        // スロットを移動する（空き場所であっても移動できるようにするのが一番バグが少ないです）
        selectedIndex = (selectedIndex + direction + activeYakitoris.Length) % activeYakitoris.Length;
        RefreshSelection();
    }

    void RefreshSelection()
    {
        for (int i = 0; i < activeYakitoris.Length; i++)
        {
            if (activeYakitoris[i] == null) continue;

            bool isSelected = (i == selectedIndex);

            if (activeYakitoris[i].TryGetComponent(out SkewerRotation rot)) rot.isSelected = isSelected;
            if (activeYakitoris[i].TryGetComponent(out YakitoriController move)) move.isSelected = isSelected;

            YakitoriUI ui = activeYakitoris[i].GetComponentInChildren<YakitoriUI>();
            if (ui != null) ui.SetHighlight(isSelected);

        }
    }

    // --- 入力判定・計算ロジックなどは以前と同じなので省略 ---
    bool GetLeftInput() => Input.GetKeyDown(KeyCode.LeftArrow) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_LEFT) ?? false);
    bool GetRightInput() => Input.GetKeyDown(KeyCode.RightArrow) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_RIGHT) ?? false);
    bool GetUpInput() => Input.GetKeyDown(KeyCode.X) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_UP) ?? false);
    bool GetDownInput() => Input.GetKeyDown(KeyCode.B) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_DOWN) ?? false);
    int CalculatePoints(float omote, float ura) { if (omote >= 150f || ura >= 150f) return 100; if (omote >= 80f && ura >= 80f) { if (Mathf.Abs(omote - 100f) < 5f && Mathf.Abs(ura - 100f) < 5f) return 2000; return 1000; } return 50; }
    void Judge(float omote, float ura) { string msg = (omote >= 150f || ura >= 150f) ? "焦げた！" : (omote >= 80f && ura >= 80f) ? "最高！" : "まだ生！"; Color col = (msg == "最高！") ? Color.blue : (msg == "焦げた！") ? Color.red : Color.cyan; EvaluationUI.Instance.ShowEvaluation(msg, col); }
    void FinishGame() { isGameActive = false; if (ScoreManager.Instance != null) ScoreManager.Instance.SaveFinalScore(); SceneManager.LoadScene("ResultScene"); }
}