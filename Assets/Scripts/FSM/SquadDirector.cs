using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct SquadComposition
{
    public int healer;
    public int attacker;
    public int defender;
}

public class SquadDirector : MonoBehaviour
{
    [Header("Team composition")]
    public SquadComposition composition;

    [Tooltip("None => a bot will be given the role of leader.")]
    public Entity leader = null;

    private int leaderCoefSquadPos = 1;

    [Header("Physic Layer")]
    [SerializeField]
    private LayerMask layerToGiveToAI;
    [SerializeField]
    private LayerMask OpponentsLayers;


    [Header("Prefabs")]
    public CustomAgent healerPrefab;
    public CustomAgent attackerPrefab;
    public CustomAgent defenderPrefab;

    public Material teamColor;

    //Squad
    public List<CustomAgent> SquadAgents { get { return allSquad; } }
    private List<CustomAgent> allSquad = new List<CustomAgent>();

    public List<CustomAgent> GetHealers { get { return healers; } }
    private List<CustomAgent> healers = new List<CustomAgent>();

    public List<CustomAgent> GetAttackers { get { return attackers; } }
    private List<CustomAgent> attackers = new List<CustomAgent>();
    public List<CustomAgent> GetDefenders { get { return defenders; } }
    private List<CustomAgent> defenders = new List<CustomAgent>();

    [Header("CallbackEvent for transition")]
    public UnityEvent OnFightEngaged;
    public UnityEvent OnAllEnemyEliminated;
    public UnityEvent OnSquadEntityKilled;

    //enemy squad
    public SquadDirector GetEnemySquad { get { return enemySquads.Count > 0 ? enemySquads[0] : null; } }
    private List<SquadDirector> enemySquads = new List<SquadDirector>();



    //healing list
    [HideInInspector]
    public List<Entity> unitNeedHeal = new List<Entity>();
    [HideInInspector]
    public List<Entity> unitNeedHealCritical = new List<Entity>();
    public float healthRatioCritical = 0.3f;

    private void Awake()
    {
        int nbInTeam = composition.healer + composition.attacker + composition.defender;
        if (nbInTeam == 0)
            return;

        if (leader)
        {
            leader.squad = this;
            leader.OnEntityKilled.AddListener(SquadEntityKilled);
            leader.OnTakeDamage.AddListener(SquadEntityTakeDamage);
            leader.OnHealDamage.AddListener(SquadEntityHealing);

            leader.SetPhysicLayerAndLayerToCheck((1 << 7), OpponentsLayers);
            leaderCoefSquadPos = nbInTeam;
        }

        Vector3 pos = Vector3.right * 3f;
        float angle = 360f / nbInTeam;

        Quaternion angleRota = Quaternion.Euler(0, angle, 0);

        //spawn all type of agent, if no leader set by editor, set default one
        for (int a = 0; a < composition.attacker; ++a)
        {
            CustomAgent agent = Instantiate(attackerPrefab, transform.position + pos, Quaternion.identity, transform);

            MeshRenderer renderer = agent.Body.transform.GetChild(0).GetComponent<MeshRenderer>();
            if (renderer)
                renderer.material = teamColor;

            if (!leader)
                leader = agent;

            agent.squad = this;

            //bind callback event
            agent.OnEntityKilled.AddListener(SquadEntityKilled);
            agent.OnTakeDamage.AddListener(SquadEntityTakeDamage);
            agent.OnHealDamage.AddListener(SquadEntityHealing);
            agent.OnEnemyInSight.AddListener(EnemyInSight);

            //set physic layer
            agent.SetPhysicLayerAndLayerToCheck(layerToGiveToAI, OpponentsLayers);

            //get ref in list
            attackers.Add(agent);
            allSquad.Add(agent);

            pos = angleRota * pos;
        }

        for (int h = 0; h < composition.healer; ++h)
        {
            CustomAgent agent = Instantiate(healerPrefab, transform.position + pos, Quaternion.identity, transform);

            MeshRenderer renderer = agent.Body.transform.GetChild(0).GetComponent<MeshRenderer>();
            if (renderer)
                renderer.material = teamColor;

            if (!leader)
                leader = agent;

            agent.squad = this;

            agent.OnEntityKilled.AddListener(SquadEntityKilled);
            agent.OnTakeDamage.AddListener(SquadEntityTakeDamage);
            agent.OnHealDamage.AddListener(SquadEntityHealing);
            agent.OnEnemyInSight.AddListener(EnemyInSight);

            agent.SetPhysicLayerAndLayerToCheck(layerToGiveToAI, OpponentsLayers);

            healers.Add(agent);
            allSquad.Add(agent);

            pos = angleRota * pos;
        }

        for (int d = 0; d < composition.defender; ++d)
        {
            CustomAgent agent = Instantiate(defenderPrefab, transform.position + pos, Quaternion.identity, transform);

            MeshRenderer renderer = agent.Body.transform.GetChild(0).GetComponent<MeshRenderer>();
            if (renderer)
                renderer.material = teamColor;

            if (!leader)
                leader = agent;

            agent.squad = this;

            agent.OnEntityKilled.AddListener(SquadEntityKilled);
            agent.OnTakeDamage.AddListener(SquadEntityTakeDamage);
            agent.OnHealDamage.AddListener(SquadEntityHealing);
            agent.OnEnemyInSight.AddListener(EnemyInSight);

            agent.SetPhysicLayerAndLayerToCheck(layerToGiveToAI, OpponentsLayers);

            defenders.Add(agent);
            allSquad.Add(agent);

            pos = angleRota * pos;
        }
    }

    public void EnemyInSight(Entity enemy)
    {
        if (enemy.health > 0f)
        {
            if (EngageFight(enemy.squad))
                enemy.squad.EngageFight(this);
        }
    }

    public bool IsFightEngagedWithThisSquad(SquadDirector enemySquad)
    {
        return enemySquads.Contains(enemySquad);
    }

    bool EngageFight(SquadDirector enemySquad)
    {
        if (!IsFightEngagedWithThisSquad(enemySquad))
        {
            enemySquads.Add(enemySquad);
            if (enemySquads.Count == 1)
                OnFightEngaged.Invoke();
            return true;
        }

        return false;
    }

    void SquadEntityTakeDamage(Entity entityDamaged)
    {
        if (entityDamaged.IsDead)
            return;

        float ratio = entityDamaged.HealthRatio;
        
        //dont heal healer or return if already in critcal list
        if (healers.Contains(entityDamaged) || unitNeedHealCritical.Contains(entityDamaged))
            return;

        if (ratio <= healthRatioCritical)
        {

            if (unitNeedHeal.Contains(entityDamaged))
            {
                unitNeedHeal.Remove(entityDamaged);
                return;
            }

            unitNeedHealCritical.Add(entityDamaged);
            foreach (CustomAgent healer in healers)
                healer.HealAlly = true;
        }
        else if (!unitNeedHeal.Contains(entityDamaged))
        {
            unitNeedHeal.Add(entityDamaged);
            foreach (CustomAgent healer in healers)
                healer.HealAlly = true;
        }
    }

    void SquadEntityHealing(Entity entityHealed)
    {
        if (entityHealed.IsDead)
            return;

        float ratio = entityHealed.HealthRatio;

        //need heal critical => need heal
        if (unitNeedHealCritical.Contains(entityHealed) && healthRatioCritical < ratio)
        {
            unitNeedHealCritical.Remove(entityHealed);
            unitNeedHeal.Add(entityHealed);
        }
        else if (ratio >= 1f)
        {
            unitNeedHeal.Remove(entityHealed);
        }

        if (unitNeedHeal.Count == 0 && unitNeedHealCritical.Count == 0)
        {
            foreach (CustomAgent healer in healers)
                healer.HealAlly = false;
        }
    }

    void SquadEntityKilled(Entity entity)
    {
        if (entity == leader)
        {
            leader = GetNewLeader(true);
            if (!leader)
            {
                foreach (SquadDirector enemySquad in enemySquads)
                    enemySquad.SquadKilled(this);

                Destroy(gameObject);
                return;
            }
        }
        else
        {
            //if player leader => ajust coef
            if (leaderCoefSquadPos > 0)
                --leaderCoefSquadPos;

            CustomAgent agentDeath = entity as CustomAgent;
            if (agentDeath)
            {
                //remove from list
                if (allSquad.Contains(agentDeath))
                    allSquad.Remove(agentDeath);

                if (defenders.Contains(agentDeath))
                    defenders.Remove(agentDeath);
                else if (attackers.Contains(agentDeath))
                    attackers.Remove(agentDeath);
                else if (healers.Contains(agentDeath))
                    healers.Remove(agentDeath);
            }
        }

        //remove from heal list
        if (unitNeedHealCritical.Contains(entity))
            unitNeedHealCritical.Remove(entity);
        else if (unitNeedHeal.Contains(entity))
            unitNeedHeal.Remove(entity);


        OnSquadEntityKilled.Invoke();
    }

    void SquadKilled(SquadDirector enemySquad)
    {
        if (enemySquads.Contains(enemySquad))
        {
            enemySquads.Remove(enemySquad);
            if (enemySquads.Count == 0)
                OnAllEnemyEliminated.Invoke();
        }
    }

    Entity GetNewLeader(bool removeFromList = false)
    {
        //if leader is an AI, remove from squad list
        if (removeFromList)
        {
            CustomAgent leaderAgent = leader as CustomAgent;

            if (leaderAgent)
                allSquad.Remove(leaderAgent);
            else
            {
                //Player dead
            }
        }

        //try get leader in attackers first
        if (attackers.Count > 0)
        {
            //if attacker 0 is leader check if there is attacker 1. => if not check next class
            //else if leader was not attacker set attacker 0 to leader role
            if (attackers[0] == leader)
            {
                attackers.RemoveAt(0);

                if (attackers.Count > 0)
                    return attackers[0];
            }
            else
                return attackers[0];
        }

        if (healers.Count > 0)
        {
            if (healers[0] == leader)
            {
                healers.RemoveAt(0);

                if (healers.Count > 0)
                    return healers[0];
            }
            else
                return healers[0];
        }

        if (defenders.Count > 0)
        {
            if (defenders[0] == leader)
            {
                defenders.RemoveAt(0);

                if (defenders.Count > 0)
                    return defenders[0];
            }
            else
                return defenders[0];
        }

        return null;
    }

    public void SetSquadState(AgentState state)
    {
        foreach (var agent in allSquad)
        {
            agent.currentState = state;
        }
    }
    
    public Vector3 GetFrontLinePos()
    {
        Vector3 pos = new Vector3();

        int i = 0;
        for (; i<defenders.Count;++i)
        {
            pos += defenders[i].transform.position;
        }
        if (i > 0)
            return pos / i;

        
        return GetSquadPos();  
    }

    public Vector3 GetSquadPos()
    {
        int nbEntity = 0;
        Vector3 pos = Vector3.zero;
        if (leader)
        {
            nbEntity = leaderCoefSquadPos;
            pos = leader.transform.position * nbEntity;

        }

        foreach (CustomAgent agent in allSquad)
        {
            if (leader == agent)
                continue;

            ++nbEntity;
            pos += agent.transform.position;
        }

        if (nbEntity > 0)
            pos /= nbEntity;

        return pos;
    }

    public void PlayerGiveTarget(Vector3 target)
    {
        foreach (CustomAgent agent in allSquad)
        {
            agent.OverrideFight = true;
            agent.target = target;
            agent.HasTarget = true;
        }
    }

    private void OnDrawGizmos()
    {
        if(leader)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(leader.transform.position + Vector3.up * 2, 0.3f);
        }
    }
}
