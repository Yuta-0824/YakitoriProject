using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class YakitoriManager : MonoBehaviour
{
    [Header("焼き鳥の設定")]
    public GameObject yakitoriPrefab;
    public Transform[] spawnPoints; // ヒエラルキーのPos1, Pos2, Pos3を登録

    [Header("UI参照")]
    public TextMeshProUGUI timerText;

    [Header("ゲーム設定")]
    public float timeRemaining = 60f;
    public float inputCooldown = 0.3f; // 連打防止時間

    // 現在場にある焼き鳥を管理するリスト
    private List<GameObject> activeYakitoris = new List<GameObject>();
    private int selectedIndex = 0;
    private float lastInputTime = 0f;
    private bool isGameActive = true;

    void Start()
    {
        activeYakitoris.Clear();
        selectedIndex = 0;
    }

    void Update()
    {
        if (!isGameActive) return;

        // --- タイマー処理 ---
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timerText != null) timerText.text = $"TIME: {timeRemaining:F0}";
        }
        else
        {
            FinishGame();
        }

        // --- 入力クールタイムチェック ---
        if (Time.time - lastInputTime < inputCooldown) return;

        HandleInputs();
    }

    void HandleInputs()
    {
        // 1. 選択切り替え (左右キー / ジョイコン左右ボタン)
        if (GetLeftInput()) { ChangeSelection(-1); lastInputTime = Time.time; }
        if (GetRightInput()) { ChangeSelection(1); lastInputTime = Time.time; }

        // 2. 焼き鳥を増やす (Xキー / ジョイコン上ボタン)
        if (GetUpInput()) { SpawnYakitori(); lastInputTime = Time.time; }

        // 3. 焼き鳥を回収 (Bキー / ジョイコン下ボタン)
        if (GetDownInput()) { CollectYakitori(); lastInputTime = Time.time; }
    }

    void SpawnYakitori()
    {
        // リスト内の空データを掃除
        activeYakitoris.RemoveAll(item => item == null);

        if (activeYakitoris.Count >= spawnPoints.Length)
        {
            Debug.LogWarning("焼き場がいっぱいです！");
            return;
        }

        // 空いているスロット（リストの末尾）に生成
        int nextPos = activeYakitoris.Count;
        GameObject newYaki = Instantiate(yakitoriPrefab, spawnPoints[nextPos].position, spawnPoints[nextPos].rotation);
        activeYakitoris.Add(newYaki);

        // 生成した個体の中にあるUIに、その個体のCoreを紐付ける
        CookingCore newCore = newYaki.GetComponent<CookingCore>();
        YakitoriUI ui = newYaki.GetComponentInChildren<YakitoriUI>();
        if (ui != null) ui.core = newCore;

        // 全体の選択状態（黄色ハイライト等）を更新
        RefreshSelection();
    }

    void CollectYakitori()
    {
        if (activeYakitoris.Count == 0) return;

        GameObject target = activeYakitoris[selectedIndex];
        if (target != null)
        {
            CookingCore core = target.GetComponent<CookingCore>();
            if (core != null)
            {
                // スコア計算と評価（表・裏の値を個別に渡す）
                int points = CalculatePoints(core.omoteProgress, core.uraProgress);
                if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(points);

                Judge(core.omoteProgress, core.uraProgress);
            }
            Destroy(target);
        }

        activeYakitoris.RemoveAt(selectedIndex);

        // 選択インデックスがはみ出さないよう調整
        if (selectedIndex >= activeYakitoris.Count)
        {
            selectedIndex = Mathf.Max(0, activeYakitoris.Count - 1);
        }

        RefreshSelection();
    }

    void RefreshSelection()
    {
        for (int i = 0; i < activeYakitoris.Count; i++)
        {
            if (activeYakitoris[i] == null) continue;
            bool isSelected = (i == selectedIndex);

            // 各スクリプトに操作権限を伝える
            if (activeYakitoris[i].TryGetComponent(out SkewerRotation rot)) rot.isSelected = isSelected;
            if (activeYakitoris[i].TryGetComponent(out YakitoriController move)) move.isSelected = isSelected;

            // UIのハイライト（矢印や文字サイズ）を更新
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

    // --- 焼き加減の精密判定ロジック ---
    int CalculatePoints(float omote, float ura)
    {
        // 1. 【焦げ】どちらか片面でも150%以上
        if (omote >= 150f || ura >= 150f) return 100;

        // 2. 【最高】両面が80%以上
        if (omote >= 80f && ura >= 80f)
        {
            // 両面とも100%に極めて近い（誤差5%以内）ならボーナス
            if (Mathf.Abs(omote - 100f) < 5f && Mathf.Abs(ura - 100f) < 5f) return 2000;
            return 1000;
        }

        // 3. 【生】それ以外
        return 50;
    }

    // --- 評価メッセージ判定 ---
    void Judge(float omote, float ura)
    {
        string msg = "";
        Color col = Color.white;

        if (omote >= 150f || ura >= 150f)
        {
            msg = "焦げすぎだ！！"; col = Color.red;
        }
        else if (omote >= 80f && ura >= 80f)
        {
            msg = "最高！おいしそう！"; col = new Color(1.0f, 0.5f, 0.0f); // オレンジ
        }
        else
        {
            msg = "まだ生だよ！"; col = Color.cyan;
        }

        if (EvaluationUI.Instance != null)
            EvaluationUI.Instance.ShowEvaluation(msg, col);
    }

    void FinishGame()
    {
        isGameActive = false;
        if (ScoreManager.Instance != null) ScoreManager.Instance.SaveFinalScore();
        SceneManager.LoadScene("ResultScene");
    }

    // --- 入力ラップ (キーボード + ジョイコン) ---
    bool GetLeftInput() => Input.GetKeyDown(KeyCode.LeftArrow) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_LEFT) ?? false);
    bool GetRightInput() => Input.GetKeyDown(KeyCode.RightArrow) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_RIGHT) ?? false);
    bool GetUpInput() => Input.GetKeyDown(KeyCode.X) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_UP) ?? false);
    bool GetDownInput() => Input.GetKeyDown(KeyCode.B) || (JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.DPAD_DOWN) ?? false);
}