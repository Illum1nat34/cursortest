using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AggregatedModifiers
{
    public float wLoot, wExtract, wFightPvP, wFightPvE, wHeal, wHide;
    public float earlyExitLootBias, greedyExitLootBias, lowHPBias;
    public float riskBias, noiseBias;
    public bool preferStealthRoutes, avoidChokePoints, allowFakeExtract, prefer1v1Duels, chaseRareLoot;
}

public static class PerkAggregator
{
    public static AggregatedModifiers Aggregate(List<Perk> perks)
    {
        var a = new AggregatedModifiers();
        foreach (var p in perks)
        {
            a.wLoot     += p.modifiers.utility.wLoot;
            a.wExtract  += p.modifiers.utility.wExtract;
            a.wFightPvP += p.modifiers.utility.wFightPvP;
            a.wFightPvE += p.modifiers.utility.wFightPvE;
            a.wHeal     += p.modifiers.utility.wHeal;
            a.wHide     += p.modifiers.utility.wHide;

            a.earlyExitLootBias += p.modifiers.thresholds.earlyExitLootBias;
            a.greedyExitLootBias += p.modifiers.thresholds.greedyExitLootBias;
            a.lowHPBias         += p.modifiers.thresholds.lowHPBias;

            a.riskBias  += p.modifiers.riskBias;
            a.noiseBias += p.modifiers.noiseBias;

            a.preferStealthRoutes |= p.modifiers.protocol.preferStealthRoutes;
            a.avoidChokePoints    |= p.modifiers.protocol.avoidChokePoints;
            a.allowFakeExtract    |= p.modifiers.protocol.allowFakeExtract;
            a.prefer1v1Duels      |= p.modifiers.protocol.prefer1v1Duels;
            a.chaseRareLoot       |= p.modifiers.protocol.chaseRareLoot;
        }
        // Кепы
        a.wLoot     = Mathf.Clamp(a.wLoot,     -2f, 2f);
        a.wExtract  = Mathf.Clamp(a.wExtract,  -2f, 2f);
        a.wFightPvP = Mathf.Clamp(a.wFightPvP, -2f, 2f);
        a.wFightPvE = Mathf.Clamp(a.wFightPvE, -2f, 2f);
        a.wHeal     = Mathf.Clamp(a.wHeal,     -2f, 2f);
        a.wHide     = Mathf.Clamp(a.wHide,     -2f, 2f);
        a.riskBias  = Mathf.Clamp(a.riskBias,  -1f, 1f);
        a.noiseBias = Mathf.Clamp(a.noiseBias, -1f, 1f);
        return a;
    }
}