using UnityEngine;

[RequireComponent(typeof(LootSpot))]
public class Binder_LootSpot : MonoBehaviour
{
    public WorldMarker marker;
    public Color lootColor = new Color(0.2f, 0.6f, 1f, 0.9f);

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
        marker.SetColor(lootColor);
        marker.SetSize(new Vector2(0.25f,0.25f));
        marker.worldOffset = new Vector3(0, 1.2f, 0);
    }

    void Sync()
    {
        var loot = GetComponent<LootSpot>();
        marker.SetTitle($"LOOT (vol {loot.lootVolume:0.00} | val {loot.lootValue:0})");
        marker.showInGame = !loot.isLooted;
    }
}