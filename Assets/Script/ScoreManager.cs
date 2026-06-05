using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI参照")]
    public TextMeshProUGUI scoreText;

    private int totalScore = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    // スコアを加算する関数
    public void AddScore(int amount)
    {
        totalScore += amount;
        UpdateScoreUI();
    }
    // ゲーム終了時や、リザルトへ行く直前にこれを呼ぶ
    public void SaveFinalScore()
    {
        // "TotalScore" という名前で保存
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {totalScore:000000}"; // 6桁表示
        }
    }
}