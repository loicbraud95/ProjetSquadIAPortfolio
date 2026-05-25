using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerComponent
{
    //Move
    private Vector2 m_MoveDir;
    private Vector2 m_MoveSteering;
    private Vector2 m_Velocity;
    public Vector2 Velocity { get { return m_Velocity; } }

    public float maxAccel = 5f;
    public float aimSpeed = 2f;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    private bool m_IsRunning = false;

    void Update()
    {
        //compute velocity (in the update for animation fluidity)
        float speed =  m_IsRunning ? runSpeed : walkSpeed ;
        if (m_Velocity.magnitude > speed)
        {
            m_Velocity.Normalize();
            m_Velocity *= speed;
        }

        m_Velocity += m_MoveSteering * Time.fixedDeltaTime;

        if (m_Velocity.magnitude < 0.1f)
            m_Velocity = Vector2.zero;

        ComputeAccel();

        //compute rotation
        ComputeRotation();
    }

    void FixedUpdate()
    {
        //apply velocity
        playerBehaviour.RB.linearVelocity = new Vector3(m_Velocity.x, playerBehaviour.RB.linearVelocity.y, m_Velocity.y);
    }

    public void ComputeRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        Vector3 dir = hit.point - transform.position;
        dir.y = 0f;

        playerBehaviour.Body.transform.rotation = Quaternion.LookRotation(dir);
    }

    public void ComputeAccel()
    {
        //based on steering movement
        Vector2 targetVelocity2D = m_MoveDir * (m_IsRunning ? runSpeed : walkSpeed);
        m_MoveSteering = targetVelocity2D - m_Velocity;
        m_MoveSteering.Normalize();
        m_MoveSteering *= maxAccel;
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        m_MoveDir = value;
        ComputeAccel();
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        if (context.started)
            m_IsRunning = true;
        else if (context.canceled)
            m_IsRunning = false;
    }
}
