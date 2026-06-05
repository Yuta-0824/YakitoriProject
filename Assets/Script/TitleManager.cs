using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("次に行くシーンの名前")]
    public string nextSceneName = "TutorialScene"; // ここを TutorialScene に書き換える

    void Update()
    {
        // ジョイコンのAボタンでも次へ進む
        if (JoyconManager.Instance != null && JoyconManager.Instance.j.Count > 0)
        {
            if (JoyconManager.Instance.j[0].GetButtonDown(Joycon.Button.DPAD_RIGHT))
            {
                StartGame();
            }
        }
    }

    // ボタンから呼び出す関数
    public void StartGame()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}