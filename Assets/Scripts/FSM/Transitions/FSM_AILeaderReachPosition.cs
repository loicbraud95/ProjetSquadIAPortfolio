using System;
using UnityEngine;

[Serializable]
public class FSM_AILeaderReachPosition : FSM_ConditionTransition
{
    public float reachDistance = 2f;
    public override bool CheckCondition(SquadDirector squad)
    {
        CustomAgent agentLeader;
        if(squad.leader && squad.leader.TryGetComponent<CustomAgent>(out agentLeader))
        {
            bool check = agentLeader.HasReachDest(reachDistance, true);
            return reverseCondition ? !check : check;
        }
        else 
        {
            return false; 
        }
    }
}
