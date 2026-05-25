using UnityEngine;

[CreateAssetMenu(menuName = "StateTree/Conditions/OverrideFight")]
public class ST_ConditionOverrideFight : ICondition
{
    public override bool Evaluate(CustomAgent agent)
    {
        return agent.OverrideFight;
    }
}
