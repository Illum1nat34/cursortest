using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IdleEscape/Loadout", fileName = "NewLoadout")]
public class Loadout : ScriptableObject
{
    [Range(1, 10)] public int maxSlots = 6;
    [Range(1, 32)] public int cpBudget = 10;

    public List<Perk> selectedPerks = new List<Perk>();

    public int CurrentCP()
    {
        int sum = 0;
        foreach (var p in selectedPerks) sum += Mathf.Max(0, p.cp);
        return sum;
    }

    public bool HasConflict(out string reason)
    {
        for (int i = 0; i < selectedPerks.Count; i++)
        {
            var a = selectedPerks[i];
            for (int j = i + 1; j < selectedPerks.Count; j++)
            {
                var b = selectedPerks[j];
                if (a.mutuallyExclusivePerkIds.Contains(b.id) || b.mutuallyExclusivePerkIds.Contains(a.id))
                {
                    reason = $"Конфликт перков: {a.displayName} ↔ {b.displayName}";
                    return true;
                }
            }
        }
        if (selectedPerks.Count > maxSlots)
        {
            reason = $"Слотов выбрано {selectedPerks.Count}/{maxSlots}";
            return true;
        }
        if (CurrentCP() > cpBudget)
        {
            reason = $"Превышен бюджет CP: {CurrentCP()}/{cpBudget}";
            return true;
        }
        reason = "";
        return false;
    }
}