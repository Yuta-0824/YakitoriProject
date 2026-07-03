using UnityEngine;

public class YakitoriVisuals : MonoBehaviour
{
    [Header("参照")]
    public CookingCore core;
    public Renderer[] meatRenderers; // ここに自動で肉が入ります

    [Header("色の設定")]
    public Color rawColor = new Color(1.0f, 0.7f, 0.7f);    // 生肉（薄ピンク）
    public Color cookedColor = new Color(0.5f, 0.2f, 0.1f); // 食べごろ（茶色）
    public Color burntColor = new Color(0.1f, 0.1f, 0.1f);  // 焦げ（ほぼ黒）

    void Start()
    {
        // --- 修正ポイント１：自動で肉パーツをすべて見つける ---
        // これにより、インスペクターで5個ドラッグする作業が不要になります
        if (meatRenderers == null || meatRenderers.Length == 0)
        {
            meatRenderers = GetComponentsInChildren<Renderer>();
        }

        // 前に出ていたエラーの原因（meatMaterialへの代入）は、
        // 複数パーツを扱う今の構成では不要なので削除しました。
    }

    void Update()
    {
        // データがない場合は何もしない
        if (core == null || meatRenderers.Length == 0) return;

        // 焼き加減を取得（0〜200%）
        float progress = core.GetTotalProgress();

        Color targetColor;

        if (progress <= 100f)
        {
            // 0% 〜 100%：生から焼き上がりへ
            float t = progress / 100f;
            targetColor = Color.Lerp(rawColor, cookedColor, t);
        }
        else
        {
            // 100% 〜 150%以上：焼き上がりから焦げへ
            float t = (progress - 100f) / 50f;
            targetColor = Color.Lerp(cookedColor, burntColor, t);
        }

        // --- 修正ポイント２：すべての肉パーツに色を適用する ---
        foreach (Renderer r in meatRenderers)
        {
            if (r == null) continue;

            // 色を変える
            r.material.color = targetColor;

            // 【UX演出】焼けていくほどツヤ(Smoothness)を出す
            if (progress > 50f && progress <= 100f)
            {
                float shininess = Mathf.InverseLerp(50f, 100f, progress) * 0.8f;
                r.material.SetFloat("_Glossiness", shininess);
            }
        }
    }
}