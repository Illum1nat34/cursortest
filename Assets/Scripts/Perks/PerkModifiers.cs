using System;
using UnityEngine;

[Serializable]
public class UtilityWeightsDelta
{
    [Range(-2f, 2f)] public float wLoot      = 0f;
    [Range(-2f, 2f)] public float wExtract   = 0f;
    [Range(-2f, 2f)] public float wFightPvP  = 0f;
    [Range(-2f, 2f)] public float wFightPvE  = 0f;
    [Range(-2f, 2f)] public float wHeal      = 0f;
    [Range(-2f, 2f)] public float wHide      = 0f;
}

[Serializable]
public class ThresholdsDelta
{
    [Range(-1f, 1f)] public float earlyExitLootBias = 0f;   // положит. -> раньше уходим
    [Range(-1f, 1f)] public float greedyExitLootBias = 0f;  // положит. -> позже уходим
    [Range(-1f, 1f)] public float lowHPBias          = 0f;  // положит. -> «низкое ХП» наступает раньше
}

[Serializable]
public class ProtocolFlags
{
    public bool preferStealthRoutes = false;
    public bool avoidChokePoints    = false;
    public bool allowFakeExtract    = false;
    public bool prefer1v1Duels      = false;
    public bool chaseRareLoot       = false;
}

[Serializable]
public class PerkModifiers
{
    public UtilityWeightsDelta utility = new UtilityWeightsDelta();
    public ThresholdsDelta thresholds = new ThresholdsDelta();
    public ProtocolFlags protocol = new ProtocolFlags();
    [Range(-1f, 1f)] public float riskBias = 0f;       // отрицательное — любим риск меньше
    [Range(-1f, 1f)] public float noiseBias = 0f;      // положительное — шум мешает меньше
}