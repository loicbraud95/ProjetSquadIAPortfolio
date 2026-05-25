using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM_IdleState : FSM_State
{
    public float radiusPatrol = 10f;
    public Vector2 minMaxTimeLookOnPosReach;

    private Dictionary<CustomAgent, float> agentReachPosTimer = new Dictionary<CustomAgent, float>();

    private Vector3 GetRandomPos()
    {
        Vector2 newDest2D = UnityEngine.Random.insideUnitCircle * radiusPatrol;

        RaycastHit hit;
        Physics.Raycast(new Vector3(newDest2D.x, 900f, newDest2D.y), Vector3.down, out hit, 1000f);
        
        return squad.leader.transform.position + new Vector3(newDest2D.x, hit.transform.position.y, newDest2D.y);
    }

    public override void EnterState()
    {
        base.EnterState();
        squad.SetSquadState(AgentState.Idle);

        foreach (CustomAgent agent in squad.SquadAgents)
        {
            if(agent.currentState != AgentState.Special)
                agent.MoveTo(GetRandomPos());
        }
        
    }

    public override void ExitState() 
    { 
        base.ExitState();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        foreach (CustomAgent agent in squad.SquadAgents)
        {
            if (agent.currentState == AgentState.Special)
                continue;

            //check if agent is arrived to pos
            if(agentReachPosTimer.ContainsKey(agent))
            {
                agentReachPosTimer[agent] -= Time.deltaTime;

                //get new pos when timer done
                if(agentReachPosTimer[agent] < 0f)
                {
                    agent.MoveTo(GetRandomPos());
                    agentReachPosTimer.Remove(agent);
                }
            }
            else if(agent.HasReachDest())
            {
                //add to list of waiting agent
                agentReachPosTimer.Add(agent, UnityEngine.Random.Range(minMaxTimeLookOnPosReach.x, minMaxTimeLookOnPosReach.y));
            }
        }
    }
}
