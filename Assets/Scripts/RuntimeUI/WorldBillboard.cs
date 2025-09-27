using UnityEngine;

[ExecuteAlways]
public class WorldBillboard : MonoBehaviour
{
    public Camera targetCamera;
    [Tooltip("Сглаживание поворота в мире (0 = мгновенно)")]
    [Range(0f,20f)] public float smooth = 8f;
    [Tooltip("Масштаб в зависимости от дистанции (0 = выключено)")]
    [Range(0f,1f)] public float scaleByDistance = 0.3f;
    public float baseSize = 1f;
    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    void LateUpdate()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (!targetCamera) return;

        // Повернуть к камере
        Vector3 dir = transform.position - targetCamera.transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            if (smooth > 0f && Application.isPlaying)
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * smooth);
            else
                transform.rotation = look;
        }

        // Масштабировать относительно расстояния (простая модель)
        if (scaleByDistance > 0f)
        {
            float d = Mathf.Max(0.1f, Vector3.Distance(transform.position, targetCamera.transform.position));
            float k = Mathf.Clamp(baseSize + Mathf.Log(d + 1f) * scaleByDistance, minSize, maxSize);
            transform.localScale = new Vector3(k, k, k);
        }
    }
}