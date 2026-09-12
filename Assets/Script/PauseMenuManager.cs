using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UIパネル参照")]
    public GameObject pauseMenuPanel; // PauseMenuPanel
    public GameObject menuWindow;     // MenuWindow (ボタンが並ぶ窓)
    public GameObject helpWindow;     // HelpWindow (操作説明の窓)

    [Header("シーン設定")]
    public string titleSceneName = "TitleScene";

    private bool isPaused = false;

    void Start()
    {
        // 開始時はすべて閉じておく
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (helpWindow != null) helpWindow.SetActive(false);
    }

    void Update()
    {
        // ESCキー または ジョイコンの「+ボタン(PLUS)」でメニューを開閉
        if (Input.GetKeyDown(KeyCode.Escape) || GetPlusButtonDown())
        {
            if (helpWindow.activeSelf)
            {
                // ヘルプ画面が開いていたら、先にヘルプだけ閉じる
                CloseHelp();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    // --- メニューの基本操作 ---

    public void OpenMenu()
    {
        isPaused = true;
        Time.timeScale = 0f; // ゲーム内時間を完全に停止
        pauseMenuPanel.SetActive(true);
        menuWindow.SetActive(true);
        helpWindow.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // ゲーム時間を再開
        pauseMenuPanel.SetActive(false);
        helpWindow.SetActive(false);
    }

    // --- ヘルプ画面の切り替え ---

    public void OpenHelp()
    {
        menuWindow.SetActive(false); // ボタン一覧を隠す
        helpWindow.SetActive(true);   // 操作説明を表示
    }

    public void CloseHelp()
    {
        helpWindow.SetActive(false);  // 操作説明を隠す
        menuWindow.SetActive(true);   // ボタン一覧に戻る
    }

    // --- タイトルへ戻る / アプリ終了 ---

    public void BackToTitle()
    {
        Time.timeScale = 1f; // 時間を必ず1に戻してからシーン移動！
        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタ実行停止
#else
        Application.Quit(); // ビルド後のアプリ終了
#endif
    }

    // ジョイコンの+ボタン検知
    bool GetPlusButtonDown()
    {
        return JoyconInput.Instance?.rightJoycon?.GetButtonDown(Joycon.Button.PLUS) ?? false;
    }
}