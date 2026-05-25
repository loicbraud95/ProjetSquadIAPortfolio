using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    protected PlayerBehaviour playerBehaviour;
       
    protected virtual void Awake()
    {
        playerBehaviour = GetComponent<PlayerBehaviour>();
    }
}
