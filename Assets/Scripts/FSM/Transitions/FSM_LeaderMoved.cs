using System;
using UnityEngine;

[Serializable]
public class FSM_LeaderMoved : FSM_ConditionTransition
{
    public float DeltaPos = 2f;
    private Vector3 lastPos;

    public override void ResetCondition(SquadDirector squad)
    {
        if(squad.leader)
            lastPos = squad.leader.transform.position;
    }

    public override bool CheckCondition(SquadDirector squad)
    {
        if(!squad.leader)
            return false;

        float dist = (lastPos - squad.leader.transform.position).magnitude;

        bool check = dist > DeltaPos; 
        return reverseCondition ? !check : check;
    }
}
