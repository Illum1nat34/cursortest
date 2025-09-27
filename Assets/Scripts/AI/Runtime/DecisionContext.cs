using UnityEngine;

[System.Serializable]
public struct DecisionContext
{
    [Range(0f,1f)] public float lootDensity;
    [Range(0f,1f)] public float risk;
    [Range(0f,1f)] public float backpackFill;
    [Range(0f,1f)] public float lootValue01;
    [Range(0f,1f)] public float exitQuietness;
    [Range(0f,1f)] public float hp;
    [Range(0f,1f)] public float ammo;
    [Range(0f,1f)] public float enemyCount;
    [Range(0f,1f)] public float mobCount;
    [Range(0f,1f)] public float timeLeft;
    public bool isNight;
}