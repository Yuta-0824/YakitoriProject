using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        int finalScore = PlayerPrefs.GetInt("TotalScore", 0);
        if (scoreText != null) scoreText.text = $"{finalScore}点";
    }

    void Update()
    {
        // --- ジョイコンの判定を追加 ---
        if (JoyconManager.Instance != null && JoyconManager.Instance.j.Count > 0)
        {
            Joycon j = JoyconManager.Instance.j[0];

            // Aボタンでリトライ
            if (j.GetButtonDown(Joycon.Button.DPAD_RIGHT)) RetryGame();

            // Bボタン (右ジョイコンの下側ボタン) でタイトルへ
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN)) BackToTitle();
        }
    }

    public void RetryGame() { SceneManager.LoadScene("SampleScene"); }
    public void BackToTitle() { SceneManager.LoadScene("TitleScene"); }
}