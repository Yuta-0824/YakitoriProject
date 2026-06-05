using UnityEngine;
using TMPro;

public class YakitoriUI : MonoBehaviour
{
    [Header("参照")]
    public CookingCore core;
    public GameObject selectArrow; // 手順1で作った矢印をここにドラッグ
    private TextMeshProUGUI textMesh;

    private bool isSelected = false;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (core == null) core = GetComponentInParent<CookingCore>();

        // 文字の初期設定：アウトラインを有効にする
        // (インスペクターでも設定できますが、コードで書いておくと確実です)
        textMesh.outlineColor = Color.black;
        textMesh.outlineWidth = 0.2f;
    }

    void Update()
    {
        if (core == null || textMesh == null) return;

        textMesh.text =
            $"<color=#FFCCCC>表: {core.omoteProgress:F0}%</color>\n" +
            $"<color=#CCCCFF>裏: {core.uraProgress:F0}%</color>";

        // 色の更新
        if (core.GetTotalProgress() >= 110f) textMesh.color = Color.red;
        else if (isSelected) textMesh.color = Color.yellow;
        else textMesh.color = Color.white;
    }

    public void SetHighlight(bool state)
    {
        isSelected = state;

        if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();

        // 1. 矢印の表示/非表示を切り替える
        if (selectArrow != null)
        {
            selectArrow.SetActive(state);
        }

        // 2. 選択中なら文字をさらに強調する
        if (isSelected)
        {
            textMesh.fontSize = 55;
            textMesh.fontStyle = FontStyles.Bold;
            textMesh.outlineWidth = 0.3f; // 選択中は縁取りを太くする
        }
        else
        {
            textMesh.fontSize = 35;
            textMesh.fontStyle = FontStyles.Normal;
            textMesh.outlineWidth = 0.2f; // 通常時
        }
    }
}