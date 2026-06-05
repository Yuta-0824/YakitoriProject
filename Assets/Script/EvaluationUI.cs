using UnityEngine;
using TMPro;
using System.Collections;

public class EvaluationUI : MonoBehaviour
{
    public static EvaluationUI Instance; // どこからでも呼べるようにする
    private TextMeshProUGUI textMesh;

    void Awake()
    {
        Instance = this;
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = ""; // 最初は空にする
    }

    // 評価を表示する関数
    public void ShowEvaluation(string message, Color color)
    {
        StopAllCoroutines(); // 前の表示をキャンセル
        textMesh.text = message;
        textMesh.color = color;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2.0f); // 2秒間表示
        textMesh.text = ""; // 消す（フェードアウト演出などを足してもOK）
    }
}