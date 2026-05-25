using System.Collections.Generic;
using UnityEngine;

public class FSM_FollowLeader : FSM_State
{
    public float followDistance = 1f;
    public int maxUnitPerLine = 5;
    public float spaceBetweenUnits = 2f;
    public float spaceBetweenLine = 2f;

    public override void EnterState()
    {
        base.EnterState();
        squad.SetSquadState(AgentState.Idle);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if(squad.leader)
        {
            Vector3 frw = squad.leader.GetVelocityForward().normalized;
            int nbAgent = squad.SquadAgents.Count - (squad.leader as CustomAgent != null ? 1 : 0);

            List<Vector3> poses = ComputeFormation.LinePoses(frw, squad.leader.transform.position - frw * followDistance,
                nbAgent, maxUnitPerLine, spaceBetweenUnits, spaceBetweenLine);

            int i = 0;
            foreach(CustomAgent agent in squad.SquadAgents)
            {
                if (squad.leader.gameObject == agent.gameObject)
                    continue;

                if(agent.currentState == AgentState.Special)
                {
                    ++i;
                    continue;
                }

                agent.MoveTo(poses[i]);
                ++i;
            }
        }
    }
}
