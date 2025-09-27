using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IdleEscape/Perk", fileName = "NewPerk")]
public class Perk : ScriptableObject
{
    [Header("Идентификатор")]
    public string id;
    public string displayName;
    [TextArea] public string description;

    [Header("Мета")]
    public PerkType type = PerkType.Пассив;
    public PerkRarity rarity = PerkRarity.Common;
    [Range(0, 4)] public int cp = 1;
    public PerkTag tags = PerkTag.None;
    public PerkSource source = PerkSource.LevelUp;

    [Header("Конфликты")]
    [Tooltip("ID перков, с которыми этот несовместим")]
    public List<string> mutuallyExclusivePerkIds = new List<string>();

    [Header("Модификаторы поведения")]
    public PerkModifiers modifiers = new PerkModifiers();
}