using UnityEngine;
using TMPro;
using DG.Tweening; // DOTweenを使うために追加

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI参照")]
    public TextMeshProUGUI scoreText;

    private int totalScore = 0;      // 内部的な本当のスコア
    private int displayedScore = 0;  // 画面に表示されている途中のスコア
    private Tween scoreTween;        // 連続加算時のバグ防止用

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI(0);
    }

    // スコアを加算する関数
    public void AddScore(int amount)
    {
        totalScore += amount;

        // もし前の動きが残っていたら止める（連続で回収した時用）
        scoreTween?.Kill();

        // displayedScore を totalScore まで 0.5秒かけて変化させる
        scoreTween = DOTween.To(() => displayedScore, x => displayedScore = x, totalScore, 0.5f)
            .SetEase(Ease.OutQuad) // 徐々にゆっくりになる動き
            .OnUpdate(() =>
            {
                // 数値が変化するたびにUIを更新
                UpdateScoreUI(displayedScore);
            });

        // おまけ：加算時に少し数字を弾ませる（パンチ演出）
        scoreText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }

    void UpdateScoreUI(int value)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {value:000000}";
        }
    }

    // シーン遷移用の保存処理
    public void SaveFinalScore()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();
    }
}