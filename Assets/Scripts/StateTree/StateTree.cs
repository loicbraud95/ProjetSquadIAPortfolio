using System.Collections;
using UnityEngine;

public class StateTree : MonoBehaviour
{
    public CustomAgent owner;

    public ST_State root;

    public ST_State currentState;

    private void Start()
    {
        root = GetComponentInChildren<ST_State>();
        owner = GetComponentInParent<CustomAgent>();

        CheckHierarchy();
    }

    public void ChangeState(ST_State nextState)
    {
        currentState = nextState;
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public IEnumerator CheckNextUpdate()
    {
        yield return null;
        CheckHierarchy();
    }

    public void CheckHierarchy()
    {
        currentState = root;

        currentState.EnterState(this);
    }
}
