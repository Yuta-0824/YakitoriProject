using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [Header("追跡するターゲット (SkewerRootを指定)")]
    public Transform target;

    [Header("串からの高さ")]
    public float yOffset = 0.5f;

    void LateUpdate()
    {
        if (target != null)
        {
            // --- 1. 位置の固定 ---
            // 親が回転しても、UIの位置だけは「ターゲットの中心 + 真上にオフセット」に強制固定する
            transform.position = target.position + Vector3.up * yOffset;
        }

        // --- 2. 向きの固定 ---
        // 常にカメラと同じ向きにする
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}