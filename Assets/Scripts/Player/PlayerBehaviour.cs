using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : Entity
{
    private Rigidbody rb;
    public Rigidbody RB { get {  return rb; } }

    public LayerMask groundMask;

    bool isShooting = false;

    bool shootInput = false;

    private PlayerInput playerInput;


    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void UiMode()
    {
        playerInput.currentActionMap.Disable();
        playerInput.SwitchCurrentActionMap("UI");
        playerInput.currentActionMap.Enable();
    }

    public void GameMode()
    {
        playerInput.currentActionMap.Disable();
        playerInput.SwitchCurrentActionMap("PlayerActions");
        playerInput.currentActionMap.Enable();
    }

    public override Vector3 GetVelocityForward()
    {
        if (rb.linearVelocity.magnitude < 1f)
            return body.transform.forward;

        return rb.linearVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if(shootInput && isShooting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity ,groundMask);
            Vector3 newTarget = new Vector3(hit.point.x, hit.point.y, hit.point.z);

            weapon.UpdateTargetPos(newTarget);
        }
    }

    public void ShootInput(InputAction.CallbackContext context)
    {
        if (context.started)
            shootInput = true;
        else if (context.canceled)
            shootInput = false;

        if (shootInput)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity,groundMask);
            Vector3 newTarget = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            if (!isShooting)
            {
                weapon.StartShooting(newTarget);
                isShooting = true;
            }
        }

        if(!shootInput && isShooting)
        {
            weapon.StopShooting();
            isShooting = false;
        }
    }

    public void TargetInput(InputAction.CallbackContext context)
    {
        bool isTargeting = context.ReadValueAsButton();

        if (isTargeting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask);
            Vector3 newTarget = new Vector3(hit.point.x, hit.point.y, hit.point.z);

            squad.PlayerGiveTarget(newTarget);
        }
    }
}
