using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class VisibilitySensor : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<Entity, bool> objectTriggered = new Dictionary<Entity, bool>();

    public UnityEvent<Entity> OnObjectSee;
    public UnityEvent<Entity> OnObjectExit;

    [HideInInspector]
    public LayerMask layersToCheck;

    public CustomAgent Agent { set { agent = value; } }
    CustomAgent agent;

    public float peripheralDistance = 4f;
    public float peripheralAngle = 180f;

    public float focusDistance = 20f;
    public float focusAngle = 40f;

    public Entity GetNearestTarget()
    {
        Entity nearestPlayer = null;
        float dist = float.MaxValue;

        Vector3 eyePos = GetEyePos();

        List<Entity> keys = new List<Entity>(objectTriggered.Keys);
        foreach (Entity entity in keys)
        {
            if (entity == null)
            {
                objectTriggered.Remove(entity);
                continue;
            }

            if (objectTriggered[entity])
            {
                float testDist = (eyePos - entity.transform.position).magnitude;
                if (testDist < dist)
                {
                    nearestPlayer = entity;
                    dist = testDist;
                }
            }
        }

        return nearestPlayer;
    }

    Vector3 GetEyePos()
    {
        return new Vector3(transform.parent.transform.position.x, transform.position.y, transform.parent.transform.position.z);
    }

    void Start()
    {
        transform.localScale = new Vector3(peripheralDistance * 2f, 2f, focusDistance);
        transform.localPosition = new Vector3(0f, 1f, focusDistance * 0.5f);

        BoxCollider col = GetComponent<BoxCollider>();

        col.excludeLayers -= layersToCheck;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 eyePos = GetEyePos();
        float eyePosDif = eyePos.y - agent.transform.position.y;

        List<Entity> keys = new List<Entity>(objectTriggered.Keys);
        foreach (Entity entity in keys)
        {
            if(entity == null)
            {
                objectTriggered.Remove(entity);
                continue;
            }

            bool see = objectTriggered[entity];

            Vector3 targetPos = entity.transform.position;
            targetPos.y += eyePosDif;

            Vector3 dirToEntity = targetPos - eyePos;
            RaycastHit hit;
            Physics.Raycast(eyePos, dirToEntity, out hit, dirToEntity.magnitude, layersToCheck);

            if (hit.transform)
            {
                bool inSigth = hit.transform.gameObject == entity.gameObject;

                objectTriggered[entity] = inSigth;

                if (see)
                {
                    if (see != inSigth)
                        OnObjectExit.Invoke(entity);
                }
                else
                {
                    if (see != inSigth)
                        OnObjectSee.Invoke(entity);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //try get entity game object (collide with body)
        if (other.transform.parent == null || other.transform.parent.parent == null)
            return;

        Entity entity;
        GameObject entityGameObject = other.transform.parent.parent.gameObject;
        if (!entityGameObject.TryGetComponent<Entity>(out entity))
            return;

        if (objectTriggered.ContainsKey(entity) || entity.health <= 0f)
            return;

        Vector3 eyePos = GetEyePos();
        Vector3 targetPos = other.transform.position;
        targetPos.y += eyePos.y - agent.transform.position.y;

        Vector3 dirToPlayer = targetPos - eyePos;
        RaycastHit hit;
        Physics.Raycast(eyePos, dirToPlayer, out hit, dirToPlayer.magnitude, layersToCheck);
        if (hit.transform)
        {
            bool inSigth = hit.transform.gameObject == entityGameObject;

            objectTriggered.Add(entity, inSigth);

            if (inSigth)
                OnObjectSee.Invoke(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //try get entity game object (collide with body)
        if (other.transform.parent == null || other.transform.parent.parent == null)
            return;

        Entity entity;
        GameObject entityGameObject = other.transform.parent.parent.gameObject;
        if (!entityGameObject.TryGetComponent<Entity>(out entity))
            return;

        //if player is in sight
        if (objectTriggered.ContainsKey(entity) && objectTriggered[entity])
            OnObjectExit.Invoke(entity);

        objectTriggered.Remove(entity);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            transform.localScale = new Vector3(peripheralDistance * 2f, 2f, focusDistance);
            transform.localPosition = new Vector3(0f, 1f, focusDistance * 0.5f);
        }
#endif

        Vector3 basePos = GetEyePos();
        foreach (var pair in objectTriggered)
        {
            if (pair.Key)
            {
                Gizmos.color = pair.Value ? Color.green : Color.red;
                Vector3 targetPos = pair.Key.transform.position;
                targetPos.y += basePos.y - agent.transform.position.y;

                Gizmos.DrawLine(basePos, targetPos);
            }
        }
    }
}
