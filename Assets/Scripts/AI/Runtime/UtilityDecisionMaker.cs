
using UnityEngine;

public class UtilityDecisionMaker : MonoBehaviour
{
    [Header("Базовые веса целей")]
    public float wLoot = 1.0f;
    public float wExtract = 0.8f;
    public float wFightPvP = 0.7f;
    public float wFightPvE = 0.5f;
    public float wHeal = 0.6f;
    public float wHide = 0.4f;

    [Header("Пороги (0..1)")]
    public float earlyExitLoot = 0.35f;
    public float greedyExitLoot = 0.75f;
    public float lowHP = 0.3f;

    [Header("Нормализация/кривые (опционально)")]
    public AnimationCurve riskCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve lootCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve exitQuietCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Результаты")]
    public UtilityGoal currentGoal = UtilityGoal.Loot;

    // [DEBUG] последние значения скоринга и контекста
    [Header("[DEBUG] Последние скора")]
    public float last_sLoot, last_sExtract, last_sFightPvP, last_sFightPvE, last_sHeal, last_sHide;
    public float last_progress, last_risk, last_lootDensity, last_exitQuiet, last_bpFill, last_lootVal;
    public float dbg_early, dbg_greedy;

    AggregatedModifiers agg = new AggregatedModifiers();

    public void ApplyAggregated(AggregatedModifiers a) { agg = a; }

    public UtilityGoal Decide(in DecisionContext ctx)
    {
        float risk = Mathf.Clamp01(ctx.risk + agg.riskBias);
        float lootDensity = Mathf.Clamp01(ctx.lootDensity);
        float exitQuiet = Mathf.Clamp01(ctx.exitQuietness);
        float bpFill = Mathf.Clamp01(ctx.backpackFill);
        float lootVal = Mathf.Clamp01(ctx.lootValue01);
        float hp = Mathf.Clamp01(ctx.hp);
        float enemies = Mathf.Clamp01(ctx.enemyCount);
        float mobs = Mathf.Clamp01(ctx.mobCount);

        float W(float baseW, float delta) => Mathf.Clamp(baseW + delta, 0f, 3f);

        float sLoot = W(wLoot, agg.wLoot) * (lootCurve.Evaluate(lootDensity) * (1f - riskCurve.Evaluate(risk)) * (1f - Mathf.Max(bpFill, lootVal)));
        float sExtractBase = (bpFill + lootVal + exitQuietCurve.Evaluate(exitQuiet)) / 3f;
        float sExtract = W(wExtract, agg.wExtract) * sExtractBase;

        float sFightPvP = W(wFightPvP, agg.wFightPvP) * Mathf.Clamp01(enemies * (1f - risk) + (hp > 0.6f ? 0.2f : -0.2f));
        float sFightPvE = W(wFightPvE, agg.wFightPvE) * Mathf.Clamp01(mobs * (1f - risk) + 0.1f);
        float sHeal = W(wHeal, agg.wHeal) * Mathf.Clamp01((lowHP + agg.lowHPBias) - hp);
        float sHide = W(wHide, agg.wHide) * Mathf.Clamp01(risk - 0.5f);

        float progress = Mathf.Max(bpFill, lootVal);
        float early = Mathf.Clamp01(earlyExitLoot + agg.earlyExitLootBias);
        float greedy = Mathf.Clamp01(greedyExitLoot + agg.greedyExitLootBias);

        if (progress >= greedy) sExtract += 0.5f;
        else if (progress >= early && risk > 0.4f) sExtract += 0.25f;

        // save debug
        last_sLoot = sLoot; last_sExtract = sExtract; last_sFightPvP = sFightPvP; last_sFightPvE = sFightPvE; last_sHeal = sHeal; last_sHide = sHide;
        last_progress = progress; last_risk = risk; last_lootDensity = lootDensity; last_exitQuiet = exitQuiet; last_bpFill = bpFill; last_lootVal = lootVal;
        dbg_early = early; dbg_greedy = greedy;

        // choose goal
        float best = sLoot;
        currentGoal = UtilityGoal.Loot;
        void Try(float score, UtilityGoal goal)
        {
            if (score > best) { best = score; currentGoal = goal; }
        }

        Try(sExtract, UtilityGoal.Extract);
        Try(sFightPvP, UtilityGoal.FightPvP);
        Try(sFightPvE, UtilityGoal.FightPvE);
        Try(sHeal, UtilityGoal.Heal);
        Try(sHide, UtilityGoal.Hide);

        return currentGoal;
    }
}
