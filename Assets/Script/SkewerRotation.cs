using UnityEngine;
public class SkewerRotation : MonoBehaviour
{
    public float flipThreshold = 90f;
    public float flipSpeed = 15f;

    [Header("デバッグ用")]
    public KeyCode debugFlipKey = KeyCode.Space; // スペースキーでひっくり返す

    private bool isFlipped = false;
    private Quaternion targetRot;
    public bool isSelected = false; // これを追加

    void Update()
    {
        if (!isSelected) return; // 選択されていない時は以下の処理（回転）をしない
        
        // --- 1. ジョイコンのリセット (Aボタン) ---
        if (JoyconInput.Instance != null && JoyconInput.Instance.GetAButton())
        {
            JoyconInput.Instance.Recenter();
        }

        // --- 2. ひっくり返し判定 (ジョイコン) ---
        if (JoyconInput.Instance != null && JoyconInput.Instance.IsHoldingZR())
        {
            float roll = JoyconInput.Instance.GetRotation().eulerAngles.z;
            if (roll > 180) roll -= 360;

            isFlipped = Mathf.Abs(roll) > flipThreshold;
        }

        // --- 3. デバッグ機能：キーボードでの強制ひっくり返し ---
        // スペースキーを押すたびに 表(false) と 裏(true) を入れ替える
        if (Input.GetKeyDown(debugFlipKey))
        {
            isFlipped = !isFlipped;
            Debug.Log($"<color=yellow>【デバッグ】ひっくり返しました。状態: {(isFlipped ? "裏" : "表")}</color>");
        }

        // --- 4. 回転の反映 ---
        targetRot = isFlipped ? Quaternion.Euler(0, 0, 180f) : Quaternion.Euler(0, 0, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * flipSpeed);
    }
}