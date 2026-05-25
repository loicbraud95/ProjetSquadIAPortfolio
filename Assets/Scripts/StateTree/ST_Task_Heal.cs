using System;
using UnityEngine;

[Serializable]
public class ST_Task_Heal : ST_Task
{
    Entity targetToHeal;
    float capLifeDanger;

    public GameObject healParticle;

    public int healForce;

    public float rangeUntilHeal;
    public float healTickDelay;
    float healTickTimer;

    bool moving = false;
    bool arrived = false;

    public override void OnExecute()
    {
        capLifeDanger = agent.squad.healthRatioCritical;
        
        bool ally = GetNearestDamagedAlly();
        if (!ally)
            return;

        healTickTimer = healTickDelay;
        targetToHeal.OnEntityKilled.AddListener(TargetDied);
        agent.OnEntityKilled.AddListener(RemoveDeadListener);

        moving = false;
        arrived = false;

        agent.OverrideFight = false;
    }

    public override void OnExit()
    {
        taskEnded = true;

        if (agent.targetEntity == null)
            agent.HasTarget = false;
    }
    public override void OnTick()
    {
        if (!agent) { taskEnded = true; return; }

        if (!targetToHeal)
        {
            bool ally = GetNearestDamagedAlly();
            if (!ally)
                return;
        }
        else
        {
            if (!moving)
            {
                Vector3 direction = (targetToHeal.transform.position - agent.transform.position).normalized;
                float distance = (targetToHeal.transform.position - agent.transform.position).magnitude;
                agent.nmAgent.SetDestination(agent.transform.position + direction * distance);
                moving = true;
            }
            else if (!arrived && agent.HasReachDest(rangeUntilHeal, true))
            {
                arrived = true;
            }

            if (arrived)
            {
                if (healTickTimer <= 0)
                {
                    Heal();
                    healTickTimer = healTickDelay;
                }
                else
                    healTickTimer -= Time.deltaTime;
            }
        }
    }

    bool GetNearestDamagedAlly()
    {
        if (agent.squad.unitNeedHealCritical.Count > 0)
        {
            targetToHeal = null;
            foreach (Entity entity in agent.squad.unitNeedHealCritical)
            {
                if (targetToHeal == null || CheckDistance(targetToHeal.transform.position, entity.transform.position))
                {
                    ChangeTarget(entity);
                }
            }
        }
        else
        {
            if (agent.squad.unitNeedHeal.Count > 0)
            {
                if (!agent.squad.unitNeedHeal.Contains(targetToHeal))
                {
                    targetToHeal = null;
                    foreach (Entity entity in agent.squad.unitNeedHeal)
                    {
                        if (targetToHeal == null || CheckDistance(targetToHeal.transform.position, entity.transform.position))
                        {
                            ChangeTarget(entity);
                        }
                    }
                }
            }
            else
            {
                OnExit();
            }
        }

        if (targetToHeal)
            return true;
        else
            return false;

    }

    bool CheckDistance(Vector3 actual, Vector3 target)
    {
        return (target - agent.transform.position).magnitude < (actual - agent.transform.position).magnitude;
    }

    void Heal()
    {
        if (targetToHeal)
        {
            GameObject.Instantiate(healParticle, targetToHeal.transform.position, Quaternion.LookRotation(targetToHeal.transform.up), targetToHeal.transform);

            targetToHeal.HealDamage(healForce);
            targetToHeal.OnHealDamage.Invoke(targetToHeal);

            moving = false;
            arrived = false;

            if (targetToHeal.HealthRatio > capLifeDanger)
            {
                GetNearestDamagedAlly();
            }
        }
        else
            GetNearestDamagedAlly();
    }

    void ChangeTarget(Entity target)
    {
        targetToHeal = target;
        moving = false;
        arrived = false;
    }

    void TargetDied(Entity target)
    {
        if(target == targetToHeal)
            GetNearestDamagedAlly();
    }

    void RemoveDeadListener(Entity self)
    {
        if (targetToHeal)
            targetToHeal.OnEntityKilled.RemoveListener(TargetDied);
    }
}
