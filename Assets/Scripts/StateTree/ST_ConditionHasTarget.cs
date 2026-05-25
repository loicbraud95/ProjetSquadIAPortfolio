using UnityEngine;

[CreateAssetMenu(menuName = "StateTree/Conditions/Has_Target")]
public class ST_ConditionHasTarget : ICondition
{
    public override bool Evaluate(CustomAgent agent)
    {
        return agent.HasTarget;
    }
}
