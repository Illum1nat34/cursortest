using UnityEngine;

[DefaultExecutionOrder(-10)]
public class FillBlackboardSimple : MonoBehaviour
{
    public BlackboardLike blackboard;
    [Header("Начальные значения (0..1)")]
    [Range(0f,1f)] public float hp = 1f;
    [Range(0f,1f)] public float ammo = 1f;
    [Range(0f,1f)] public float baseRisk = 0.2f;

    [Header("Таймер матча")]
    public float matchSeconds = 300f;
    float time;

    void Awake()
    {
        if(!blackboard) blackboard = GetComponent<BlackboardLike>();
        time = matchSeconds;
    }

    void Update()
    {
        if(!blackboard) return;
        time = Mathf.Max(0f, time - Time.deltaTime);
        float t = matchSeconds <= 0.01f ? 0f : (time / matchSeconds);

        blackboard.Set01(BB.HP, hp);
        blackboard.Set01(BB.Ammo, ammo);
        blackboard.Set01(BB.TimeLeft, t);

        // Риск от источников шума
        float risk = baseRisk;
        var pos = transform.position;
        for(int i=0;i<NoiseZone.All.Count;i++)
        {
            var nz = NoiseZone.All[i];
            float d = Vector3.Distance(pos, nz.transform.position);
            if(d < nz.radius)
            {
                float k = 1f - (d / nz.radius); // ближе -> больше риск
                risk = Mathf.Max(risk, Mathf.Clamp01(nz.riskValue * k));
            }
        }
        blackboard.Set01(BB.Risk, risk);

        // Тишина выхода ~ обратная риска (упрощённо для прототипа)
        blackboard.Set01(BB.ExitQuietness, 1f - risk);
    }
}