using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "Scriptable Objects/WeaponInfo")]
public class WeaponInfo : ScriptableObject
{
    public int magCapacity;
    public float fireRate;
    public float reloadDuration;
    public float damage;
    public float range;
    public float maxSpray;
}
