using UnityEngine;

public class YakitoriController : MonoBehaviour
{
    public bool isSelected = false; // Managerから制御されるフラグ

    [Header("移動スピード")]
    public float moveSpeed = 10f;

    [Header("移動制限の範囲")]
    public float minX = -8f; // 左の限界
    public float maxX = 6f;  // 右の限界

    [Header("持ち上げ設定")]
    public float liftHeight = 1.5f;
    public float liftSpeed = 10f;

    private float currentX; // 現在のX座標を保持
    private float targetY;  // 目標の高さ
    private Vector3 initialPos;

    void Start()
    {
        // 最初に配置された場所のX座標を記憶
        currentX = transform.localPosition.x;
        initialPos = transform.localPosition;
    }

    void Update()
    {
        if (!isSelected) return; // 選択されていない時は動かさない
        if (Time.timeScale == 0) return; // 一時停止中は動かさない

        HandleMovement();
    }

    void HandleMovement()
    {
        // --- 1. 左右の移動入力を取得 ---
        float horizontalInput = Input.GetAxis("Horizontal"); // キーボード A/D または 矢印

        if (JoyconInput.Instance != null && JoyconInput.Instance.rightJoycon != null)
        {
            // ジョイコンのスティック入力を加算
            horizontalInput += JoyconInput.Instance.rightJoycon.GetStick()[0];
        }

        // --- 2. X座標の計算と制限（Clamp） ---
        // 入力があった分だけ currentX を増減させる
        currentX += horizontalInput * moveSpeed * Time.deltaTime;

        // 指定された -8 ～ 6 の範囲から出ないように固定する
        currentX = Mathf.Clamp(currentX, minX, maxX);


        // --- 3. 上下の判定（持ち上げ） ---
        bool liftInput = false;
        if (JoyconInput.Instance != null && JoyconInput.Instance.rightJoycon != null)
        {
            float pitch = JoyconInput.Instance.GetRotation().eulerAngles.x;
            if (pitch > 180) pitch -= 360;
            if (pitch < -30f) liftInput = true;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) liftInput = true;

        targetY = liftInput ? initialPos.y + liftHeight : initialPos.y;


        // --- 4. 座標を反映 ---
        // Xは計算した currentX、Yは目標の高さ、Zは元の位置
        Vector3 nextPos = new Vector3(currentX, targetY, transform.localPosition.z);

        // 滑らかに移動させる（少しキビキビ動くようにLerpの係数を強めにしています）
        transform.localPosition = Vector3.Lerp(transform.localPosition, nextPos, Time.deltaTime * 15f);
    }
}