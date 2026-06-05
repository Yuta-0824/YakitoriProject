using UnityEngine;
using UnityEngine.Video; // 動画機能に必要
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "MainScene"; // ゲーム本編のシーン名

    void Start()
    {
        if (videoPlayer != null)
        {
            // 動画が最後まで再生された時のイベントを登録
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void Update()
    {
        // --- ジョイコンまたはキーボードでのスキップ判定 ---
        if (IsSkipPressed())
        {
            GoToGameScene();
        }
    }

    bool IsSkipPressed()
    {
        // 1. キーボードのスペースキー
        if (Input.GetKeyDown(KeyCode.Space)) return true;

        // 2. ジョイコンのAボタン（DPAD_RIGHT）
        if (JoyconManager.Instance != null && JoyconManager.Instance.j.Count > 0)
        {
            if (JoyconManager.Instance.j[0].GetButtonDown(Joycon.Button.DPAD_RIGHT)) return true;
        }

        return false;
    }

    // 動画が自然に終わった時に呼ばれる
    void OnVideoFinished(VideoPlayer vp)
    {
        GoToGameScene();
    }

    // ボタンから直接呼ぶ用
    public void GoToGameScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}