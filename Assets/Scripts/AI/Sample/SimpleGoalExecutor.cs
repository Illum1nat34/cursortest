using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleGoalExecutor : MonoBehaviour
{
    public BlackboardLike blackboard;
    public UtilityDecisionMaker decision;

    [Header("Поведение")]
    public float interactDistance = 1.2f;
    public float openLootSeconds = 1.0f;

    [Header("Нормализация ценности")]
    [Tooltip("При какой суммарной ценности считаем, что уже ‘много’. Используется для LootValue01 (0..1)")]
    public float lootValueForNormalization = 200f;

    [Header("Поиск пути")]
    [Tooltip("Максимальный радиус семплинга целевой точки на NavMesh")]
    public float sampleRadius = 2.0f;
    [Tooltip("Отбрасывать частичные пути (NavMeshPathStatus.Partial)")]
    public bool rejectPartialPaths = true;

    NavMeshAgent agent;
    UtilityGoal lastGoal = UtilityGoal.Loot;
    LootSpot currentLoot;
    ExitZone currentExit;

    LootSpot[] lootSpots;
    ExitZone[] exits;

    float carriedLootValue = 0f;

    NavMeshPath _tmpPath; // переиспользуем для экономии GC

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!blackboard) blackboard = GetComponent<BlackboardLike>();
        if (!decision) decision = GetComponent<UtilityDecisionMaker>();
        lootSpots = GameObject.FindObjectsOfType<LootSpot>();
        exits = GameObject.FindObjectsOfType<ExitZone>();
        _tmpPath = new NavMeshPath();
        UpdateLootDensity();
        blackboard.Set01(BB.LootValue01, 0f);
    }

    void Update()
    {
        var goalStr = blackboard.GetString(BB.CurrentGoal, "Loot");
        if (System.Enum.TryParse<UtilityGoal>(goalStr, out var g))
        {
            if (g != lastGoal) SwitchGoal(g);
            lastGoal = g;
        }

        switch (lastGoal)
        {
            case UtilityGoal.Loot: TickLoot(); break;
            case UtilityGoal.Extract: TickExtract(); break;
            default: TickLoot(); break;
        }
    }

    void SwitchGoal(UtilityGoal g)
    {
        if (g == UtilityGoal.Loot) currentExit = null;
        if (g == UtilityGoal.Extract) currentLoot = null;
    }

    void TickLoot()
    {
        if (!currentLoot || currentLoot.isLooted)
        {
            currentLoot = FindNearestLootByPath();
            if (currentLoot)
                agent.SetDestination(GetNavmeshPoint(currentLoot.transform.position));
        }
        else
        {
            float d = Vector3.Distance(transform.position, currentLoot.transform.position);
            if (d <= interactDistance)
            {
                StartCoroutine(OpenLootThenFill());
            }
        }
    }

    IEnumerator OpenLootThenFill()
    {
        var loot = currentLoot;
        currentLoot = null;
        agent.isStopped = true;
        yield return new WaitForSeconds(openLootSeconds);
        agent.isStopped = false;

        if (loot && !loot.isLooted)
        {
            loot.isLooted = true;
            float fill = blackboard.Get01(BB.BackpackFill, 0f);
            fill = Mathf.Clamp01(fill + loot.lootVolume);
            blackboard.Set01(BB.BackpackFill, fill);

            carriedLootValue += Mathf.Max(0f, loot.lootValue);
            float value01 = lootValueForNormalization <= 0.01f ? 1f : Mathf.Clamp01(carriedLootValue / lootValueForNormalization);
            blackboard.Set01(BB.LootValue01, value01);

            UpdateLootDensity();
        }
    }

    void TickExtract()
    {
        if (!currentExit)
        {
            currentExit = FindNearestExitByPath();
            if (currentExit)
                agent.SetDestination(GetNavmeshPoint(currentExit.transform.position));
        }
        else
        {
            float d = Vector3.Distance(transform.position, currentExit.transform.position);
            if (d <= currentExit.interactRadius)
            {
                Debug.Log("<color=lime>Extracted!</color>  LootValue=" + carriedLootValue.ToString("0") + ", BackpackFill=" + blackboard.Get01(BB.BackpackFill).ToString("0.00"));
                enabled = false;
                agent.isStopped = true;
            }
        }
    }

    // --- NEW: выбор ближайшей цели по длине пути по навмешу (и проверка достижимости)
    LootSpot FindNearestLootByPath()
    {
        LootSpot best = null;
        float bestLen = float.MaxValue;
        Vector3 start = transform.position;

        for (int i = 0; i < lootSpots.Length; i++)
        {
            var l = lootSpots[i];
            if (!l || l.isLooted) continue;

            float len;
            if (!TryPathLength(start, l.transform.position, out len)) continue;
            if (len < bestLen)
            {
                bestLen = len;
                best = l;
            }
        }
        return best;
    }

    ExitZone FindNearestExitByPath()
    {
        ExitZone best = null;
        float bestLen = float.MaxValue;
        Vector3 start = transform.position;

        for (int i = 0; i < exits.Length; i++)
        {
            var e = exits[i];
            if (!e) continue;

            float len;
            if (!TryPathLength(start, e.transform.position, out len)) continue;
            if (len < bestLen)
            {
                bestLen = len;
                best = e;
            }
        }
        return best;
    }

    bool TryPathLength(Vector3 from, Vector3 to, out float length)
    {
        length = float.PositiveInfinity;

        Vector3 a = GetNavmeshPoint(from);
        Vector3 b = GetNavmeshPoint(to);

        if (!NavMesh.CalculatePath(a, b, NavMesh.AllAreas, _tmpPath)) return false;
        if (_tmpPath.status == NavMeshPathStatus.PathInvalid) return false;
        if (rejectPartialPaths && _tmpPath.status == NavMeshPathStatus.PathPartial) return false;

        float dist = 0f;
        var corners = _tmpPath.corners;
        if (corners == null || corners.Length < 2) return false;
        for (int i = 1; i < corners.Length; i++)
            dist += Vector3.Distance(corners[i - 1], corners[i]);

        if (float.IsInfinity(dist) || float.IsNaN(dist)) return false;
        length = dist;
        return true;
    }

    Vector3 GetNavmeshPoint(Vector3 worldPos)
    {
        if (NavMesh.SamplePosition(worldPos, out var hit, sampleRadius, NavMesh.AllAreas))
            return hit.position;
        return worldPos; // fallback
    }

    void UpdateLootDensity()
    {
        if (lootSpots == null || lootSpots.Length == 0) { blackboard.Set01(BB.LootDensity, 0f); return; }
        int remaining = 0;
        for (int i = 0; i < lootSpots.Length; i++) if (lootSpots[i] && !lootSpots[i].isLooted) remaining++;
        float density = Mathf.Clamp01((float)remaining / lootSpots.Length);
        blackboard.Set01(BB.LootDensity, density);
    }
}
