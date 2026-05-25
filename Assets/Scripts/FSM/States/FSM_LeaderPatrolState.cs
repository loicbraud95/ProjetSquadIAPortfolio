using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FSM_LeaderPatrolState : FSM_State
{
    public List<GameObject> patrolPoint = new List<GameObject>();

    private int currentPathPoint = -1;
    private bool loopFront = true;

    public UnityEvent OnPositionGive;

    public override void EnterState()
    {
        base.EnterState();
        squad.SetSquadState(AgentState.Idle);

        if (patrolPoint.Count < 2)
        {
            Debug.Log(name + " need at least 2 control path");
            return;
        }

        CustomAgent agent;
        //logic patrol
        if (squad.leader.TryGetComponent<CustomAgent>(out agent))
        {
            if (loopFront)
            {
                if (currentPathPoint + 1 >= patrolPoint.Count)
                {
                    loopFront = false;
                    --currentPathPoint;
                }
                else
                    ++currentPathPoint;
            }
            else
            {
                if (currentPathPoint - 1 < 0)
                {
                    loopFront = true;
                    ++currentPathPoint;
                }
                else
                    --currentPathPoint;
            }

            SetNewDest(agent);
        }
    }
    private void SetNewDest(CustomAgent agent)
    {
        if (agent)
        {
            agent.MoveTo(patrolPoint[currentPathPoint].transform.position);
            OnPositionGive.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (GameObject point in patrolPoint)
        {
            if (point)
                Gizmos.DrawWireSphere(point.transform.position, 0.5f);
        }
    }
}
