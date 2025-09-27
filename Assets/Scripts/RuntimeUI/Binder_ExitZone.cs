using UnityEngine;

[RequireComponent(typeof(ExitZone))]
public class Binder_ExitZone : MonoBehaviour
{
    public WorldMarker marker;
    public Color exitColor = new Color(0.1f, 1f, 0.1f, 0.9f);

    void Reset(){ TryBind(); }
    void OnValidate(){ TryBind(); }
    void Awake(){ TryBind(); }
    void Update(){ if(marker) marker.SetTitle("EXIT"); }

    void TryBind()
    {
        if (!marker)
        {
            marker = GetComponent<WorldMarker>();
            if (!marker) marker = gameObject.AddComponent<WorldMarker>();
        }
        marker.SetColor(exitColor);
        marker.SetSize(new Vector2(0.22f,0.22f));
        marker.worldOffset = new Vector3(0, 1.3f, 0);
    }
}