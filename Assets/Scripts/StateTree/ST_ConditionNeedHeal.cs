using UnityEngine;

[CreateAssetMenu(menuName = "StateTree/Conditions/NeedHeal")]
public class ST_ConditionNeedHeal : ICondition
{
    public override bool Evaluate(CustomAgent agent)
    {
        return agent.HealAlly;
    }
}
