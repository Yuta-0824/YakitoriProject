using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mySkewer; // 自分の親を格納する

    [Header("高さの調整")]
    public float yOffset = 0.5f;

    void Start()
    {
        // 自動的に自分の「親オブジェクト（SkewerRoot）」を取得する
        mySkewer = transform.parent;
    }

    void LateUpdate()
    {
        if (mySkewer != null)
        {
            // 1. 位置の固定：親の場所 + 真上にyOffset
            // 親が回転しても、UIは常に「世界座標の真上」に配置されます
            transform.position = mySkewer.position + Vector3.up * yOffset;
        }

        // 2. 向きの固定：常にカメラと同じ向きにする
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}