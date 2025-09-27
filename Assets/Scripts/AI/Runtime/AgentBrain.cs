
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentBrain : MonoBehaviour
{
    public Loadout loadout;
    public UtilityDecisionMaker decision;
    public BlackboardLike blackboard;

    [Header("Отладка")]
    public DecisionContext lastCtx;
    public UtilityGoal lastGoal;

    void Awake()
    {
        if (!decision) decision = GetComponent<UtilityDecisionMaker>();
        if (!blackboard) blackboard = GetComponent<BlackboardLike>();
        ApplyPerks();
    }

    public void ApplyPerks()
    {
        if (!decision) decision = GetComponent<UtilityDecisionMaker>();
        if (!loadout || decision == null) return;

        if (loadout.HasConflict(out var reason))
        {
            Debug.LogWarning($"Loadout конфликтует: {reason}");
        }

        var agg = PerkAggregator.Aggregate(loadout.selectedPerks);
        decision.ApplyAggregated(agg);
    }

    void Update()
    {
        if (!blackboard || !decision) return;

        var ctx = new DecisionContext
        {
            lootDensity   = blackboard.Get01(BB.LootDensity),
            risk          = blackboard.Get01(BB.Risk),
            backpackFill  = blackboard.Get01(BB.BackpackFill),
            lootValue01   = blackboard.Get01(BB.LootValue01),
            exitQuietness = blackboard.Get01(BB.ExitQuietness),
            hp            = blackboard.Get01(BB.HP),
            ammo          = blackboard.Get01(BB.Ammo),
            enemyCount    = blackboard.Get01(BB.EnemyCount),
            mobCount      = blackboard.Get01(BB.MobCount),
            timeLeft      = blackboard.Get01(BB.TimeLeft),
            isNight       = blackboard.GetBool(BB.IsNight)
        };
        lastCtx = ctx;
        lastGoal = decision.Decide(ctx);
        blackboard.SetString(BB.CurrentGoal, lastGoal.ToString());
    }
}
