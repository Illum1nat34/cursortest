using UnityEngine;
using System.Collections.Generic;

public class NoiseZone : MonoBehaviour
{
    public static readonly List<NoiseZone> All = new List<NoiseZone>();
    [Range(0f,1f)] public float riskValue = 0.7f;
    [Range(1f,30f)] public float radius = 8f;

    void OnEnable(){ if(!All.Contains(this)) All.Add(this); }
    void OnDisable(){ All.Remove(this); }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoStyles.NoiseFill;
        Gizmos.DrawSphere(transform.position, Mathf.Min(0.35f, radius * 0.1f));
        #if UNITY_EDITOR
        UnityEditor.Handles.color = GizmoStyles.NoiseWire;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);
        var style = new GUIStyle(UnityEditor.EditorStyles.boldLabel);
        style.normal.textColor = new Color(1f,0.5f,0.1f,1f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.75f, $"NOISE ({riskValue:0.00})", style);
        #endif
    }
}