using UnityEngine;

[RequireComponent(typeof(NoiseZone))]
public class Binder_NoiseZone : MonoBehaviour
{
    public WorldMarker marker;
    public Color noiseColor = new Color(1f, 0.5f, 0.1f, 0.9f);

    void Reset(){ TryBind(); }
    void OnValidate(){ TryBind(); }
    void Awake(){ TryBind(); }
    void Update(){ Sync(); }

    void TryBind()
    {
        if (!marker)
        {
            marker = GetComponent<WorldMarker>();
            if (!marker) marker = gameObject.AddComponent<WorldMarker>();
        }
        marker.SetColor(noiseColor);
        marker.SetSize(new Vector2(0.2f,0.2f));
        marker.worldOffset = new Vector3(0, 1.1f, 0);
    }

    void Sync()
    {
        var nz = GetComponent<NoiseZone>();
        marker.SetTitle($"NOISE ({nz.riskValue:0.00})");
    }
}