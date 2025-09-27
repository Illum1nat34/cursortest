using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class WorldMarker : MonoBehaviour
{
    [Header("Настройки")]
    public string title = "MARKER";
    public Color color = new Color(0.2f, 0.6f, 1f, 0.85f);
    public Vector2 size = new Vector2(0.25f, 0.25f);
    public Vector3 worldOffset = new Vector3(0, 1.4f, 0);
    public bool showInGame = true;

    [Header("Ссылки (создаются автоматически)")]
    public Canvas canvas;
    public Image icon;
    public Text label;
    public WorldBillboard billboard;

    void OnEnable() { EnsureBuilt(); RefreshVisual(); }
    void OnValidate() { EnsureBuilt(); RefreshVisual(); }
    void Update()
    {
        if (canvas) canvas.enabled = showInGame;
        if (canvas) canvas.transform.position = transform.position + worldOffset;
    }

    public void SetTitle(string t) { title = t; RefreshVisual(); }
    public void SetColor(Color c) { color = c; RefreshVisual(); }
    public void SetSize(Vector2 s) { size = s; RefreshVisual(); }

    void EnsureBuilt()
    {
        if (!canvas)
        {
            // Canvas
            var go = new GameObject($"MarkerCanvas_{gameObject.name}", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 5000; // поверх
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();

            // Billboard
            billboard = go.AddComponent<WorldBillboard>();
            billboard.baseSize = 1f;
            billboard.scaleByDistance = 0.25f;

            // Background/Icon
            var iconGO = new GameObject("Icon", typeof(RectTransform));
            iconGO.transform.SetParent(go.transform, false);
            icon = iconGO.AddComponent<Image>();
            icon.raycastTarget = false;

            // Label
            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(go.transform, false);
            label = labelGO.AddComponent<Text>();
            label.alignment = TextAnchor.MiddleCenter;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.raycastTarget = false;
        }
    }

    void RefreshVisual()
    {
        if (!canvas) return;
        canvas.transform.position = transform.position + worldOffset;

        if (icon)
        {
            icon.color = color;
            var rt = icon.rectTransform;
            rt.sizeDelta = size * 100f; // 1 unit ~= 100px в WorldSpace Canvas
            rt.anchoredPosition = Vector2.zero;
        }
        if (label)
        {
            label.text = title;
            label.color = new Color(color.r*0.75f, color.g*0.75f, color.b*0.75f, 1f);
            var rt = label.rectTransform;
            rt.sizeDelta = new Vector2(size.x * 130f, 24f);
            rt.anchoredPosition = new Vector2(0, size.y * 65f + 12f);
            label.fontSize = 18;
        }
    }
}