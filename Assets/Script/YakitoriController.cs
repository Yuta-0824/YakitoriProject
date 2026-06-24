using UnityEngine;

public class YakitoriController : MonoBehaviour
{
    public bool isSelected = false;

    [Header("移動設定")]
    public float moveSpeed = 10f;
    public float minX = -8f;
    public float maxX = 6f;
    public float liftHeight = 1.5f;

    [Header("当たり判定の設定")]
    public float yakitoriWidth = 2.0f; // 焼き鳥の横幅（これ以上近づけなくなる距離）

    private float currentX;
    private float targetY;
    private Vector3 initialPos;
    private YakitoriManager manager; // マネージャーへの参照

    void Start()
    {
        currentX = transform.localPosition.x;
        initialPos = transform.localPosition;
        // シーン内のマネージャーを探す
        manager = FindObjectOfType<YakitoriManager>();
    }

    void Update()
    {
        if (!isSelected || Time.timeScale == 0) return;

        HandleMovement();
    }

    void HandleMovement()
    {
        // 1. 入力を取得
        float horizontalInput = Input.GetAxis("Horizontal");
        if (JoyconInput.Instance != null && JoyconInput.Instance.rightJoycon != null)
        {
            horizontalInput += JoyconInput.Instance.rightJoycon.GetStick()[0];
        }

        // 2. 移動したい量を計算
        float moveAmount = horizontalInput * moveSpeed * Time.deltaTime;
        float nextX = currentX + moveAmount;

        // 3. 【重要】他の焼き鳥とぶつからないかチェック
        if (manager != null)
        {
            // マネージャーが持っている全焼き鳥の配列をチェック
            foreach (GameObject otherYaki in manager.activeYakitoris)
            {
                // 自分自身、または既に消されたやつはスキップ
                if (otherYaki == null || otherYaki == this.gameObject) continue;

                float otherX = otherYaki.transform.localPosition.x;

                // もし移動先が、他の焼き鳥の「横幅」以内に入ってしまうなら
                if (Mathf.Abs(nextX - otherX) < yakitoriWidth)
                {
                    // 移動をキャンセル（今の位置から動かさない）
                    moveAmount = 0;
                    break;
                }
            }
        }

        // 4. 最終的なX座標を更新し、壁の範囲内に制限
        currentX += moveAmount;
        currentX = Mathf.Clamp(currentX, minX, maxX);

        // --- 以下、持ち上げと座標反映は同じ ---
        bool liftInput = false;
        if (JoyconInput.Instance?.rightJoycon != null)
        {
            float pitch = JoyconInput.Instance.GetRotation().eulerAngles.x;
            if (pitch > 180) pitch -= 360;
            if (pitch < -30f) liftInput = true;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) liftInput = true;

        targetY = liftInput ? initialPos.y + liftHeight : initialPos.y;
        Vector3 nextPos = new Vector3(currentX, targetY, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, nextPos, Time.deltaTime * 15f);
    }
}