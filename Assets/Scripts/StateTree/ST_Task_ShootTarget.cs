using System;
using UnityEngine;

[Serializable]
public class ST_Task_ShootTarget : ST_Task
{
    Entity targetEntity;

    public override void OnExecute()
    {
        targetEntity = agent.targetEntity;
        agent.weapon.StartShooting(targetEntity.transform.position);
        targetEntity.OnEntityKilled.AddListener(EntityDied);
        agent.nmAgent.updateRotation = false;
        agent.OnEntityKilled.AddListener(OwnerDied);
    }
    public override void OnExit()
    {
        taskEnded = true;
        targetEntity = null;
        agent.weapon.StopShooting();
        agent.nmAgent.updateRotation = true;
    }
    public override void OnTick()
    {
        if (agent.targetEntity == null)
        {
            OnExit();
            return;
        }

        targetEntity = agent.targetEntity;
        agent.weapon.UpdateTargetPos(targetEntity.transform.position);
        agent.transform.rotation = Quaternion.LookRotation(new Vector3(targetEntity.transform.position.x, agent.transform.position.y, targetEntity.transform.position.z) - agent.transform.position);

        if (agent.OverrideFight || agent.HealAlly)
        {
            OnExit();
        }
    }

    public void EntityDied(Entity diedEntity)
    {
        if (diedEntity == targetEntity)
        {
            targetEntity = null;
            OnExit();
        }
    }

    public void OwnerDied(Entity diedEntity)
    {
        if(targetEntity)
        {
            targetEntity.OnEntityKilled.RemoveListener(EntityDied);
        }
    }
}
