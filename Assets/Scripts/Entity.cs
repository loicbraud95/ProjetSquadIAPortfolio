using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [HideInInspector]
    public SquadDirector squad;

    [HideInInspector]
    public float bodyHeightPos = 1f;

    public float HealthRatio { get { return health / maxHealth; } }
    public float health = 100;

    private float maxHealth = 100;
    public Weapon weapon;

    [HideInInspector]
    public bool IsDead = false;



    [HideInInspector]
    public UnityEvent<Entity> OnEntityKilled;

    [HideInInspector]
    public UnityEvent<Entity> OnTakeDamage;

    [HideInInspector]
    public UnityEvent<Entity> OnHealDamage;

    public GameObject Body {  get { return body; } }
    [SerializeField]
    protected GameObject body;

    protected virtual void Awake()
    {
        bodyHeightPos = body.transform.localPosition.y;
        maxHealth = health;
        weapon.owner = this;


        OnEntityKilled.AddListener(SetDeath);
    }

    public virtual void SetPhysicLayerAndLayerToCheck(LayerMask selfLayer, LayerMask layerToCheck)
    {
        int layer = (int)Mathf.Log(selfLayer.value, 2);
        body.layer = layer;

        for (int child = 0; child < body.transform.childCount; child++)
            body.transform.GetChild(child).gameObject.layer = layer;

        weapon.enemyCharacters = layerToCheck;
    }

    public virtual Vector3 GetVelocityForward()
    {
        return Vector3.zero;
    }

    public void SetDeath(Entity self)
    {
        if (IsDead)
            return;

        IsDead = true;
        Destroy(gameObject, 0.1f);
        gameObject.SetActive(false);
    }

    public virtual void TakeDamage(float healthPoint, Entity damageDealer)
    {
        health -= healthPoint;
        if(health <= 0)
        {
            OnEntityKilled.Invoke(this);
        }
    }

    public virtual void HealDamage(float healthPoint)
    {
        health += healthPoint;

        if (health > maxHealth)
            health = maxHealth;
    }
}
