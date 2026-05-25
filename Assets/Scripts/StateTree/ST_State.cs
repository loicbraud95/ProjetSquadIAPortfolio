using System.Collections.Generic;
using UnityEngine;

public class ST_State : MonoBehaviour
{
    [SerializeReference,SubclassSelector] public List<ST_Task> tasks = new List<ST_Task>();
    public List<ICondition> conditions = new List<ICondition>();
    public List<ST_Transition> transitions = new List<ST_Transition>();

    [HideInInspector]
    public List<ST_State> childrens = new List<ST_State>();

    bool allTaskEnded = false;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            childrens.Add(child.gameObject.GetComponent<ST_State>());
        }
    }

    public void EnterState(StateTree stateTree)
    {
        if (tasks.Count <= 0)
        {
            ExitState(stateTree);
        }
        else
        {
            foreach (ST_Task task in tasks)
            {
                task.taskEnded = false;
                task.agent = stateTree.owner;
                task.OnExecute();
            }
        }
    }

    public void ExitState(StateTree stateTree)
    {
        foreach (ST_Transition t in transitions)
        {
            if (t.Condition.Evaluate(stateTree.owner))
            {
                stateTree.currentState = t.TargetState;
                t.TargetState.EnterState(stateTree);
                return;
            }
        }

        if (childrens.Count > 0)
        {
            foreach(ST_State child in childrens)
            {
                if(child.CheckCondition(stateTree.owner))
                {
                    stateTree.currentState = child;
                    child.EnterState(stateTree);
                    return;
                }
            }
        }
        stateTree.currentState = null;
        StartCoroutine(stateTree.CheckNextUpdate());

    }

    public void UpdateState(StateTree stateTree)
    {
        allTaskEnded = true;
        foreach (ST_Task task in tasks)
        {
            if (!task.taskEnded)
            {
                task.OnTick();
                allTaskEnded = false;
            }
        }

        if (allTaskEnded)
        {
            ExitState(stateTree);
        }
    }

    private bool CheckCondition(CustomAgent owner)
    {
        bool allCondition = true;
        foreach (ICondition condition in conditions)
        {
            if (!condition.Evaluate(owner))
            {
                allCondition = false;
                break;
            }
        }
        return allCondition;
    }
}
