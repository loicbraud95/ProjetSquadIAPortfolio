using System;
using UnityEngine;

[Serializable]
public class ST_Task_Infinite : ST_Task
{
    public override void OnExecute()
    {
    }
    public override void OnExit()
    {
        taskEnded = true;
    }
    public override void OnTick()
    {
    }

    public void WakeUp()
    {
        OnExit();
    }
}
