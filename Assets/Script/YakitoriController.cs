using UnityEngine;

public class YakitoriController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 10f;
    public float horizontalRange = 2.5f; // 左右に動ける幅
    public float liftHeight = 1.5f;      // 持ち上げる高さ

    private Vector3 basePos; // 基準となる中心位置
    private float targetX = 0f;
    private bool isLifted = false;

    void Start()
    {
        // 起動時の位置をセンターとして記録
        basePos = transform.localPosition;
    }
    public bool isSelected = false; // これを追加

    void Update()
    {
        if (!isSelected) return; // 選択されていない時は以下の処理（回転）をしない
        // --- 1. 左右の移動判定 (ジョイコン + キーボード) ---
        float horizontalInput = 0f;

        // ジョイコンのスティック入力を取得
        if (JoyconInput.Instance != null && JoyconInput.Instance.rightJoycon != null)
        {
            horizontalInput = JoyconInput.Instance.rightJoycon.GetStick()[0];
        }

        // キーボード入力 (矢印キー または A/Dキー) を加算
        // Input.GetAxis("Horizontal") は標準で矢印キーに対応しています
        horizontalInput += Input.GetAxis("Horizontal");

        // 入力値を -1.0 ～ 1.0 の間に制限して、移動目標を計算
        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        targetX = horizontalInput * horizontalRange;


        // --- 2. 上下の移動判定 (ジョイコン + キーボード) ---
        bool liftInput = false;

        // ジョイコンの傾き判定
        if (JoyconInput.Instance != null && JoyconInput.Instance.rightJoycon != null)
        {
            float pitch = JoyconInput.Instance.GetRotation().eulerAngles.x;
            if (pitch > 180) pitch -= 360;
            if (pitch < -30f) liftInput = true;
        }

        // キーボードの「上矢印キー」または「Wキー」でも持ち上げられるようにする(デバッグ用)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            liftInput = true;
        }


        // --- 3. 目標座標の計算と移動 ---
        Vector3 targetPos = basePos;
        targetPos.x += targetX;
        if (liftInput) targetPos.y += liftHeight;

        // Lerpを使ってスムーズに移動させる
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * moveSpeed);
    }
}