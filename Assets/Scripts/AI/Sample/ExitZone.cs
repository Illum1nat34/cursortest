using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [Range(0.5f, 3f)] public float interactRadius = 1.2f;

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        UnityEditor.Handles.color = GizmoStyles.ExitWire;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, interactRadius);
        var style = new GUIStyle(UnityEditor.EditorStyles.boldLabel);
        style.normal.textColor = new Color(0.1f,1f,0.1f,1f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.75f, "EXIT", style);
        #endif
        Gizmos.color = GizmoStyles.ExitFill;
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.15f, 0.18f);
    }
}