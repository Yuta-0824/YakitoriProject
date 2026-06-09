using UnityEngine;
using TMPro;

public class YakitoriUI : MonoBehaviour
{
    [Header("参照")]
    public CookingCore core;
    public GameObject selectArrow;
    private TextMeshProUGUI textMesh;

    private bool isSelected = false;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (core == null) core = GetComponentInParent<CookingCore>();

        // 文字を中央揃え、アウトラインを設定
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.outlineColor = Color.black;
        textMesh.outlineWidth = 0.25f;
    }

    void Update()
    {
        if (core == null || textMesh == null) return;

        // --- 表示内容の整形 ---
        // \nで改行し、表・裏を色分けして表示
        textMesh.text =
            $"<color=#FFB2B2>表 {core.omoteProgress:F0}%</color>\n" +
            $"<color=#B2B2FF>裏 {core.uraProgress:F0}%</color>";

        // --- 色の更新（110%以上で全体を赤く、選択中なら黄色） ---
        if (core.GetTotalProgress() >= 110f)
        {
            textMesh.color = Color.red;
        }
        else if (isSelected)
        {
            textMesh.color = Color.yellow;
        }
        else
        {
            textMesh.color = Color.white;
        }
    }

    public void SetHighlight(bool state)
    {
        isSelected = state;

        // --- 修正ポイント：textMeshが空（null）なら、その場で取得する ---
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        // 矢印の表示切り替え（selectArrowがInspectorで未設定でもエラーにならないようガード）
        if (selectArrow != null)
        {
            selectArrow.SetActive(state);
        }

        // --- ここが55行目付近：textMeshが確実にある状態で実行する ---
        if (textMesh != null)
        {
            if (isSelected)
            {
                textMesh.fontSize = 55;
                textMesh.fontStyle = FontStyles.Bold;
                textMesh.outlineWidth = 0.3f;
            }
            else
            {
                textMesh.fontSize = 35;
                textMesh.fontStyle = FontStyles.Normal;
                textMesh.outlineWidth = 0.2f;
            }
        }
    }
}