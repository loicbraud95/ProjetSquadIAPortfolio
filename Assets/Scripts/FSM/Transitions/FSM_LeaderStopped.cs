using System;
using UnityEngine;

[Serializable]
public class FSM_LeaderStopped : FSM_ConditionTransition
{
    public float TimeToStay = 3f;
    private float currentTimeToStay = 0f;

    public float DistanceToStayPoint = 2f;

    private Vector3 position;

    public override void ResetCondition(SquadDirector squad)
    {
        position = squad.leader.transform.position;
        currentTimeToStay = TimeToStay;
    }

    public override bool CheckCondition(SquadDirector squad)
    {
        if((position - squad.leader.transform.position).magnitude < DistanceToStayPoint)
        {
            currentTimeToStay -= Time.deltaTime;
        }
        else
        {
            position = squad.leader.transform.position;
            currentTimeToStay = TimeToStay;
        }


            bool check = currentTimeToStay <= 0f;

        return reverseCondition ? !check : check;
    }
}
