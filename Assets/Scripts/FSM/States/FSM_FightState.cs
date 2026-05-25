using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM_FightState : FSM_State
{
    [Header("Defender property")]
    public float defenderDistToEnemySquad = 5f;
    public float defendersDistToSquad = 5f;

    [Range(1, 10)]
    public int maxDefenderPerLine = 3;

    public float angleBetweenDefender = 20f;
    public float increaseAnglePerLine = 10f;

    [Range(0, 1)]
    public float defendersDecreaseDistFactorPerLine = 0.6f;

    [Header("Attacker property")]
    public float attackerDistToHealers = 5f;

    [Range(1, 10)]
    public int maxAttackerPerLine = 3;
    public float spaceBetweenAttacker = 2f;
    public float spaceBetweenAttackerLine = 2f;

    [Header("Healer property")]

    public float healerDistToDefenders = 2f;
    [Range(1, 10)]
    public int maxHealerPerLine = 3;
    public float spaceBetweenHealer = 2f;
    public float spaceBetweenHealerLine = 2f;

    [Header("Debug")]
    public int defenderCountDebug = 3;
    public int attackerCountDebug = 3;
    public int healerCountDebug = 3;
    public override void EnterState()
    {
        base.EnterState();
        squad.SetSquadState(AgentState.Combat);

        Vector3 posMoy = squad.GetSquadPos();
        Vector3 enemyPosMoy = squad.GetEnemySquad.GetSquadPos();
        Vector3 dirFight = enemyPosMoy - posMoy;

        foreach (CustomAgent agent in squad.SquadAgents)
        {
            agent.nmAgent.updateRotation = false;
            agent.transform.rotation = Quaternion.LookRotation(dirFight);
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        Vector3 posMoy = squad.GetSquadPos();
        Vector3 enemyPosMoy = squad.GetEnemySquad.GetSquadPos();
        Vector3 dirFight = enemyPosMoy - posMoy;

        Vector3 enemyFrontLinePos = squad.GetEnemySquad.GetFrontLinePos();

        //if defenders to close to enemy squad => move defenders pos
        float distDefToEnemy = (enemyFrontLinePos - ((dirFight.normalized * defendersDistToSquad) + posMoy)).magnitude;
        if (distDefToEnemy < defenderDistToEnemySquad)
            posMoy -= dirFight.normalized * (defenderDistToEnemySquad - distDefToEnemy);

        //Compute defenders poses
        List<Vector3> defendersPoses = ComputeFormation.CerclePoses(dirFight.normalized, posMoy, defendersDistToSquad, defendersDecreaseDistFactorPerLine, squad.GetDefenders.Count, maxDefenderPerLine, 0, angleBetweenDefender, increaseAnglePerLine);

        List<CustomAgent> defenders = squad.GetDefenders;
        for (int i = 0; i < squad.GetDefenders.Count; ++i)
        {
            if (defenders[i].currentState != AgentState.Special)
                defenders[i].MoveTo(defendersPoses[i]);

            if (defenders[i].targetEntity == null)
            {
                defenders[i].transform.rotation = Quaternion.LookRotation(dirFight);
                defenders[i].targetEntity = defenders[i].visibilitySensor.GetNearestTarget();
                if (defenders[i].targetEntity != null)
                    defenders[i].HasTarget = true;
            }
        }

        //get nearest defender to place attackers 
        Vector3 nearestDef = defendersPoses[defendersPoses.Count - 1];

        posMoy = nearestDef - dirFight.normalized * healerDistToDefenders;

        //Compute healers poses
        List<Vector3> healerPoses = ComputeFormation.LinePoses(dirFight.normalized, posMoy, squad.GetHealers.Count, maxHealerPerLine, spaceBetweenHealer, spaceBetweenHealerLine);
        List<CustomAgent> healers = squad.GetHealers;
        for (int i = 0; i < healers.Count; ++i)
        {
            if (healers[i].currentState != AgentState.Special)
                healers[i].MoveTo(healerPoses[i]);

            if (healers[i].targetEntity == null)
            {
                healers[i].transform.rotation = Quaternion.LookRotation(dirFight);
                healers[i].targetEntity = healers[i].visibilitySensor.GetNearestTarget();
                if (healers[i].targetEntity != null)
                    healers[i].HasTarget = true;
            }
        }
        Vector3 nearestHealer = healerPoses[healerPoses.Count - 1];
        posMoy = nearestHealer - dirFight.normalized * attackerDistToHealers;

        //Compute attackers poses
        List<Vector3> attackerPoses = ComputeFormation.LinePoses(dirFight.normalized, posMoy, squad.GetAttackers.Count, maxAttackerPerLine, spaceBetweenAttacker, spaceBetweenAttackerLine);

        List<CustomAgent> attackers = squad.GetAttackers;
        for (int i = 0; i < attackers.Count; ++i)
        {
            if (attackers[i].currentState != AgentState.Special)
                attackers[i].MoveTo(attackerPoses[i]);

            if (attackers[i].targetEntity == null)
            {
                attackers[i].transform.rotation = Quaternion.LookRotation(dirFight);
                attackers[i].targetEntity = attackers[i].visibilitySensor.GetNearestTarget();
                if (attackers[i].targetEntity != null)
                    attackers[i].HasTarget = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (squad == null || squad.GetEnemySquad == null)
            return;

        Vector3 posMoy = squad.GetSquadPos();
        Vector3 enemyPosMoy = squad.GetEnemySquad.GetSquadPos();
        Vector3 enemyDir = enemyPosMoy - posMoy;

        Vector3 enemyFrontLinePos = squad.GetEnemySquad.GetFrontLinePos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(posMoy, new Vector3(2f, 5f, 2f));

        //defenders poses
        float distDefToEnemy = (enemyFrontLinePos - ((enemyDir.normalized * defendersDistToSquad) + posMoy)).magnitude;
        if (distDefToEnemy < defenderDistToEnemySquad)
        {
            posMoy -= enemyDir.normalized * (defenderDistToEnemySquad - distDefToEnemy);
        }

        List<Vector3> defendersPoses = ComputeFormation.CerclePoses(enemyDir.normalized, posMoy, defendersDistToSquad, defendersDecreaseDistFactorPerLine, defenderCountDebug, maxDefenderPerLine, 0, angleBetweenDefender, increaseAnglePerLine);
        Gizmos.color = Color.blue;
        for (int i = 0; i < defenderCountDebug; ++i)
        {
            Gizmos.DrawSphere(defendersPoses[i], 0.5f);
        }

        Vector3 nearestDef = defendersPoses[defendersPoses.Count - 1];
        Gizmos.color = Color.orange;
        Gizmos.DrawCube(nearestDef, new Vector3(1.2f, 6f, 1.2f));

        //healers poses
        posMoy = nearestDef - enemyDir.normalized * healerDistToDefenders;

        List<Vector3> healerPoses = ComputeFormation.LinePoses(enemyDir.normalized, posMoy, healerCountDebug, maxHealerPerLine, spaceBetweenHealer, spaceBetweenHealerLine);
        Gizmos.color = Color.green;
        for (int i = 0; i < healerCountDebug; ++i)
        {
            Gizmos.DrawSphere(healerPoses[i], 0.5f);
        }

        Vector3 nearestHealer = healerPoses[healerPoses.Count - 1];
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawCube(nearestHealer, new Vector3(1.2f, 6f, 1.2f));

        //attackers poses
        posMoy = nearestHealer - enemyDir.normalized * attackerDistToHealers;

        List<Vector3> attackerPoses = ComputeFormation.LinePoses(enemyDir.normalized, posMoy, attackerCountDebug, maxAttackerPerLine, spaceBetweenAttacker, spaceBetweenAttackerLine);
        Gizmos.color = Color.red;
        for (int i = 0; i < attackerCountDebug; ++i)
        {
            Gizmos.DrawSphere(attackerPoses[i], 0.5f);
        }

        Vector3 nearestAttacker = attackerPoses[attackerPoses.Count - 1];
        Gizmos.color = Color.darkOrange;
        Gizmos.DrawCube(nearestAttacker, new Vector3(1.2f, 6f, 1.2f));
    }
}
