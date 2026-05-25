using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector]
    public Entity owner;
    
    public WeaponInfo weaponInfo;

    public GameObject damageParticle;

    bool shooting;
    [HideInInspector]
    public int currentMag;
    float shotCooldown;
    float reloadCooldown;

    Vector3 target;

    public Transform shotPoint;

    public LayerMask enemyCharacters;

    private void Start()
    {
        shotCooldown = 0;
        currentMag = weaponInfo.magCapacity;
        reloadCooldown = weaponInfo.reloadDuration;
    }

    public void StartShooting(Vector3 position)
    {
        shooting = true;
        target = new Vector3(position.x, position.y + owner.bodyHeightPos, position.z);
    }

    public void StopShooting()
    {
        shooting = false;
    }

    private void Update()
    {
        if (currentMag > 0)
        {
            if (shotCooldown < 0)
            {
                if (shooting)
                {
                    shotCooldown = weaponInfo.fireRate;
                    currentMag -= 1;
                    Shoot();
                }
            }
            else
                shotCooldown -= Time.deltaTime;
        }
        else
        {
            if (reloadCooldown < 0)
            {
                currentMag = weaponInfo.magCapacity;
                reloadCooldown = weaponInfo.reloadDuration;
            }
            else
                reloadCooldown -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        //Random spray

        float randomDirection = Random.Range(-weaponInfo.maxSpray, weaponInfo.maxSpray);

        Vector3 forward = (target - shotPoint.position).normalized;

        Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;

        Vector3 shot = target + right * randomDirection;

        //Shot logic
        RaycastHit hit;
        Ray ray = new Ray(shotPoint.position, shot - shotPoint.position);
        bool didHit = Physics.Raycast(ray, out hit, weaponInfo.range, enemyCharacters);
        if (didHit)
        {
            //try get entity game object (collide with body)
            if (hit.collider.transform.parent != null && hit.collider.transform.parent.parent != null)
            {
                Entity hitEntity;
                if (hit.collider.transform.parent.parent.TryGetComponent<Entity>(out hitEntity))
                {
                    hitEntity.TakeDamage(weaponInfo.damage, owner);
                    hitEntity.OnTakeDamage.Invoke(hitEntity);

                    GameObject particle = Instantiate(damageParticle, hit.point, Quaternion.LookRotation(hit.normal), hitEntity.transform);
                }
            }
        }

        //visuel
        if(didHit)
            RayViewer.Instance.AddRay(ray,hit.distance);
        else
            RayViewer.Instance.AddRay(ray);
    }

    public void UpdateTargetPos(Vector3 position)
    {
        target = new Vector3(position.x, position.y + owner.bodyHeightPos, position.z);
    }
}
