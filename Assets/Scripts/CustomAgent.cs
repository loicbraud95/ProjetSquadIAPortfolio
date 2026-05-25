using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum AgentState
{
    Idle,
    Combat,
    Special
}

public class CustomAgent : Entity
{
    [HideInInspector]
    public AgentState currentState;

    public NavMeshAgent nmAgent;
    StateTree stateTree;

    [HideInInspector]
    public UnityEvent<Entity> OnEnemyInSight;

    public VisibilitySensor visibilitySensor;

    public bool OverrideFight = false;
    
    public bool HasTarget = false;

    public bool HealAlly = false;

    public Vector3 target;
    public Entity targetEntity;

    public HealthBar healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();
        stateTree = GetComponentInChildren<StateTree>();
        visibilitySensor.Agent = this;

        visibilitySensor.OnObjectSee.AddListener(OnEnemyInSight.Invoke);
    }

    public override Vector3 GetVelocityForward()
    {
        if(nmAgent.velocity.magnitude < 1f)
            return body.transform.forward;

        return nmAgent.velocity;
    }

    public override void SetPhysicLayerAndLayerToCheck(LayerMask selfLayer, LayerMask layerToCheck)
    {
        base.SetPhysicLayerAndLayerToCheck(selfLayer, layerToCheck);

        visibilitySensor.layersToCheck = layerToCheck;
    }

    public void MoveTo(Vector3 dest)
    {
        nmAgent.isStopped = false;
        nmAgent.SetDestination(dest);
    }
    public void StopMove()
    {
        nmAgent.isStopped = true;
    }

    public bool HasReachDest(float radius = -1f, bool realDest = false)
    {
        return ((realDest ? GetRealDestination() : GetDestination()) - nmAgent.transform.position).magnitude < ((radius < 0) ? nmAgent.stoppingDistance : radius);
    }

    public Vector3 GetDestination()
    { return nmAgent.pathEndPosition; }

    public Vector3 GetRealDestination()
    { return nmAgent.destination; }


    public override void TakeDamage(float healthPoint, Entity damageDealer)
    {
        if (IsDead)
            return;

        base.TakeDamage(healthPoint, damageDealer);

        healthBar.UpdateBar(HealthRatio);

        //if no target => target = damage dealer
        if (targetEntity == null)
        {
            targetEntity = damageDealer;
            HasTarget = true;

            if (!squad.IsFightEngagedWithThisSquad(damageDealer.squad))
                squad.EnemyInSight(damageDealer);
        }
        //if damage dealer closer then target => target = damage dealer
        else if((targetEntity.transform.position - transform.position).magnitude > (damageDealer.transform.position - transform.position).magnitude)
        {
            targetEntity = damageDealer;
        }
    }

    public override void HealDamage(float healthPoint)
    {
        if (IsDead)
            return;

        base.HealDamage(healthPoint);
        healthBar.UpdateBar(HealthRatio);
    }
}
