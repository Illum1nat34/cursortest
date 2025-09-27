using UnityEngine;

public class LootSpot : MonoBehaviour
{
    [Header("Loot Parameters")]
    [Tooltip("Насколько заполняет рюкзак (0..1 добавки)")]
    [Range(0f,1f)] public float lootVolume = 0.2f;
    [Tooltip("Ценность лута в условных единицах")]
    [Range(0f,1000f)] public float lootValue = 50f;

    [Tooltip("Можно ли лутать повторно")]
    public bool singleUse = true;
    [HideInInspector] public bool isLooted = false;

    private void OnDrawGizmos()
    {
        var c = isLooted ? new Color(0.2f,0.8f,0.2f,0.28f) : new Color(0.2f, 0.6f, 1f, 0.35f);
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.25f, 0.3f);
        #if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(0.2f, 0.6f, 1f, 0.9f);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, 0.45f);
        var style = new GUIStyle(UnityEditor.EditorStyles.boldLabel);
        style.normal.textColor = new Color(0.15f,0.55f,1f,1f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.75f, $"LOOT (vol {lootVolume:0.00} | val {lootValue:0})", style);
        #endif
    }
}