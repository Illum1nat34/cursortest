
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class DebugHUD : MonoBehaviour
{
    public BlackboardLike blackboard;
    public UtilityDecisionMaker decision;
    public bool visible = true;
    public KeyCode toggleKey = KeyCode.F1;

    GUIStyle _label, _value, _title;
    bool _stylesReady = false;

    void Awake()
    {
        if (!blackboard) blackboard = FindObjectOfType<BlackboardLike>();
        if (!decision) decision = FindObjectOfType<UtilityDecisionMaker>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) visible = !visible;
    }

    void EnsureStyles()
    {
        if (_stylesReady) return;
        // Инициализируем стили без обращения к GUI.skin вне OnGUI
        _title = new GUIStyle();
        _title.fontSize = 18;
        _title.fontStyle = FontStyle.Bold;
        _title.normal.textColor = Color.white;

        _label = new GUIStyle();
        _label.fontSize = 14;
        _label.normal.textColor = Color.white;

        _value = new GUIStyle();
        _value.fontSize = 14;
        _value.fontStyle = FontStyle.Bold;
        _value.normal.textColor = Color.white;

        _stylesReady = true;
    }

    void OnGUI()
    {
        if (!visible) return;
        EnsureStyles();

        const int pad = 10;
        int x = pad, y = pad;

        DrawBox(x, y, 320, 280, "AI Debug HUD");
        GUILayout.BeginArea(new Rect(x + 10, y + 28, 300, 240));

        if (decision)
        {
            GUILayout.Label("Goal: " + decision.currentGoal, _title);
        }

        if (blackboard)
        {
            DrawKV("LootDensity", blackboard.Get01(BB.LootDensity).ToString("0.00"));
            DrawKV("Risk", blackboard.Get01(BB.Risk).ToString("0.00"));
            DrawKV("BackpackFill", blackboard.Get01(BB.BackpackFill).ToString("0.00"));
            DrawKV("LootValue01", blackboard.Get01(BB.LootValue01).ToString("0.00"));
            DrawKV("ExitQuietness", blackboard.Get01(BB.ExitQuietness).ToString("0.00"));
            DrawKV("HP", blackboard.Get01(BB.HP).ToString("0.00"));
            DrawKV("TimeLeft", blackboard.Get01(BB.TimeLeft).ToString("0.00"));
        }

        if (decision)
        {
            GUILayout.Space(6);
            GUILayout.Label("Scores:", _label);
            DrawKV("sLoot", decision.last_sLoot.ToString("0.000"));
            DrawKV("sExtract", decision.last_sExtract.ToString("0.000"));
            DrawKV("sFightPvP", decision.last_sFightPvP.ToString("0.000"));
            DrawKV("sFightPvE", decision.last_sFightPvE.ToString("0.000"));
            DrawKV("sHeal", decision.last_sHeal.ToString("0.000"));
            DrawKV("sHide", decision.last_sHide.ToString("0.000"));

            GUILayout.Space(6);
            GUILayout.Label("Progress / Thresholds:", _label);
            DrawKV("progress(max(fill,val))", decision.last_progress.ToString("0.00"));
            DrawKV("early", decision.dbg_early.ToString("0.00"));
            DrawKV("greedy", decision.dbg_greedy.ToString("0.00"));
        }

        GUILayout.EndArea();
    }

    void DrawBox(int x, int y, int w, int h, string title)
    {
        var prev = GUI.color;
        GUI.color = new Color(0,0,0,0.55f);
        GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(x + 10, y + 6, w - 20, 20), title, _title);
        GUI.color = prev;
    }

    void DrawKV(string k, string v)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(k, _label, GUILayout.Width(150));
        GUILayout.Label(v, _value);
        GUILayout.EndHorizontal();
    }
}
