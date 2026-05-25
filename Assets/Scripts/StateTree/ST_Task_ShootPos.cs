using System;
using UnityEngine;

[Serializable]
public class ST_Task_ShootPos : ST_Task
{
    public float maxCooldown;
    float cooldown;

    Vector3 target;

    public override void OnExecute()
    {
        cooldown = maxCooldown;
        target = agent.target;
        agent.weapon.StartShooting(target);
        agent.nmAgent.updateRotation = false;
        agent.currentState = AgentState.Special;
        agent.OverrideFight = false;
    }
    public override void OnExit()
    {
        taskEnded = true;
        agent.weapon.StopShooting();
        agent.nmAgent.updateRotation = true;
        agent.currentState = agent.targetEntity ? AgentState.Combat : AgentState.Idle;

        if(!agent.targetEntity)
            agent.HasTarget = false;

        agent.OverrideFight = false;
    }
    public override void OnTick()
    {
        cooldown -= Time.deltaTime;

        target = agent.target;
        agent.weapon.UpdateTargetPos(target);
        agent.transform.rotation = Quaternion.LookRotation(new Vector3(target.x, agent.transform.position.y, target.z) - agent.transform.position);

        if (cooldown <= 0 || agent.HealAlly)
            OnExit();
    }
}
