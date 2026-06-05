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

    // 現在場にある焼き鳥を管理するリスト
    public List<GameObject> activeYakitoris = new List<GameObject>();
    private int selectedIndex = 0;
    private float lastInputTime = 0f;
    private bool isGameActive = true;

    void Start()
    {
        // --- 修正ポイント：開始時にリストを完全に空にする ---
        activeYakitoris.Clear();
        selectedIndex = 0;
    }

    void Update()
    {
        if (!isGameActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timerText != null) timerText.text = $"TIME: {timeRemaining:F0}";
        }
        else { FinishGame(); }

        if (Time.time - lastInputTime < inputCooldown) return;

        HandleInputs();

        // 【デバッグ】現在のリストの状態をインスペクター以外でも確認できるようにする（Lキー）
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"現在リストに登録されている本数: {activeYakitoris.Count}");
            for (int i = 0; i < activeYakitoris.Count; i++)
            {
                Debug.Log($"インデックス {i}: {activeYakitoris[i]?.name}");
            }
        }
    }

    void HandleInputs()
    {
        if (JoyconInput.Instance == null) return;

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
        // --- この1行が抜けていませんか？ ---
        // 現在リストに何本あるかを数えて、次に出す場所（番号）を決める
        int nextPos = activeYakitoris.Count;

        if (nextPos >= spawnPoints.Length)
        {
            Debug.LogWarning("焼き場がいっぱいです！");
            return;
        }

        // 生成処理
        GameObject newYaki = Instantiate(yakitoriPrefab, spawnPoints[nextPos].position, spawnPoints[nextPos].rotation);

        // 生成した「この一本(newYaki)」の中にあるUIを探して、Coreをセットする
        CookingCore newCore = newYaki.GetComponent<CookingCore>();
        YakitoriUI ui = newYaki.GetComponentInChildren<YakitoriUI>();

        if (ui != null && newCore != null)
        {
            ui.core = newCore;
        }

        activeYakitoris.Add(newYaki);
        RefreshSelection();
    }
    void CollectYakitori()
    {
        if (activeYakitoris.Count == 0) return;

        // nullチェックを入れて安全に削除
        if (activeYakitoris[selectedIndex] != null)
        {
            GameObject target = activeYakitoris[selectedIndex];
            CookingCore core = target.GetComponent<CookingCore>();
            if (core != null)
            {
                float avg = core.GetTotalProgress();
                ScoreManager.Instance.AddScore(CalculatePoints(avg, core.omoteProgress, core.uraProgress));
                Judge(avg);
            }
            Destroy(target); // 物体を消す
        }

        activeYakitoris.RemoveAt(selectedIndex); // リストの名簿から消す

        // インデックスがはみ出さないように調整
        if (selectedIndex >= activeYakitoris.Count)
        {
            selectedIndex = Mathf.Max(0, activeYakitoris.Count - 1);
        }

        RefreshSelection();
    }

    // 全体の選択状態を一括で更新する
    void RefreshSelection()
    {
        for (int i = 0; i < activeYakitoris.Count; i++)
        {
            if (activeYakitoris[i] == null) continue;
            bool isSelected = (i == selectedIndex);

            if (activeYakitoris[i].TryGetComponent(out SkewerRotation rot)) rot.isSelected = isSelected;
            if (activeYakitoris[i].TryGetComponent(out YakitoriController move)) move.isSelected = isSelected;

            YakitoriUI ui = activeYakitoris[i].GetComponentInChildren<YakitoriUI>();
            if (ui != null) ui.SetHighlight(isSelected);
        }
    }

    void ChangeSelection(int direction)
    {
        if (activeYakitoris.Count <= 1) return;
        selectedIndex = (selectedIndex + direction + activeYakitoris.Count) % activeYakitoris.Count;
        RefreshSelection();
    }

    // --- 入力判定のラップ（読みやすくするため） ---
    bool GetLeftInput() => Input.GetKeyDown(KeyCode.LeftArrow) || (JoyconInput.Instance.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_LEFT) ?? false);
    bool GetRightInput() => Input.GetKeyDown(KeyCode.RightArrow) || (JoyconInput.Instance.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_RIGHT) ?? false);
    bool GetUpInput() => Input.GetKeyDown(KeyCode.X) || (JoyconInput.Instance.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_UP) ?? false);
    bool GetDownInput() => Input.GetKeyDown(KeyCode.B) || (JoyconInput.Instance.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_DOWN) ?? false);

    // --- 以下、計算・評価・終了処理は以前と同じ ---
    int CalculatePoints(float avg, float omote, float ura) { if (Mathf.Approximately(avg, 100f)) return 2000; if (avg > 80 && avg < 110) return 1000; return (avg >= 110) ? 100 : 50; }
    void Judge(float score) { string msg = score > 80 && score < 110 ? "最高！" : (score >= 110 ? "焦げた！" : "生だよ！"); Color col = score > 80 && score < 110 ? Color.yellow : (score >= 110 ? Color.red : Color.cyan); EvaluationUI.Instance.ShowEvaluation(msg, col); }
    void FinishGame() { isGameActive = false; ScoreManager.Instance.SaveFinalScore(); SceneManager.LoadScene("ResultScene"); }
}