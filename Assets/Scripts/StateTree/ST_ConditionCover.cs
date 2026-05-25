using UnityEngine;

[CreateAssetMenu(menuName = "StateTree/Conditions/Cover")]
public class ST_ConditionCover : ICondition
{
    public override bool Evaluate(CustomAgent agent)
    {
        return agent.targetEntity == null;
    }
}
