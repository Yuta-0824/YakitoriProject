using UnityEngine;

public class CookingCore : MonoBehaviour
{
    [Header("焼き状態")]
    public float omoteProgress = 0f;
    public float uraProgress = 0f;
    public float baseCookSpeed = 10f; // 基本の速さ
    public bool isOverFire = false;

    [Header("距離による設定")]
    public float maxHeatDistance = 1.5f; // 火の力が届く最大距離
    public float currentDistanceFactor = 0f; // 現在の熱効率 (0〜1.0) 表示用

    private Transform currentFireTransform; // 当たっている火の場所

    void Update()
    {
        if (isOverFire && currentFireTransform != null)
        {
            UpdateCookingWithDistance();
        }
        else
        {
            currentDistanceFactor = 0f;
        }
    }

    void UpdateCookingWithDistance()
    {
        // 1. 火の中心と肉の「3次元的な距離」を計算
        // transform.position は肉の位置、currentFireTransform.position は火の中心
        float distance = Vector3.Distance(transform.position, currentFireTransform.position);

        // 2. 距離を 0～1 の係数に変換
        // 火の中心(距離0)なら 1.0、maxHeatDistance以上離れたら 0.0 になる
        currentDistanceFactor = Mathf.InverseLerp(maxHeatDistance, 0f, distance);

        // 3. 向きの判定
        float dot = Vector3.Dot(transform.up, Vector3.up);

        // 4. 最終的な焼きスピード計算
        float finalSpeed = baseCookSpeed * currentDistanceFactor;

        if (dot > 0.1f) uraProgress += finalSpeed * Time.deltaTime;
        else if (dot < -0.1f) omoteProgress += finalSpeed * Time.deltaTime;

        omoteProgress = Mathf.Clamp(omoteProgress, 0, 200);
        uraProgress = Mathf.Clamp(uraProgress, 0, 200);
    }

    // 当たった火の情報を保存する
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            isOverFire = true;
            currentFireTransform = other.transform; // 火の場所を記録
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            isOverFire = false;
            currentFireTransform = null;
        }
    }

    public float GetTotalProgress() => (omoteProgress + uraProgress) / 2f;
}