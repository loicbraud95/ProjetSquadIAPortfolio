using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform followPoint;

    public float speed;

    void Update()
    {
        if(followPoint)
            transform.position += (followPoint.position - transform.position) * speed;
    }
}
