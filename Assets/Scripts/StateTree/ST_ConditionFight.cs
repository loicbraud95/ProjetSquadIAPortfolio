using UnityEngine;

[CreateAssetMenu(menuName = "StateTree/Conditions/Fight")]
public class ST_ConditionFight : ICondition
{
    public override bool Evaluate(CustomAgent agent)
    {
        return agent.targetEntity != null;
    }
}
