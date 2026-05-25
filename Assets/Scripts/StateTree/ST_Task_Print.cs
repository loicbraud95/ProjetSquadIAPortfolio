using System;
using UnityEngine;

[Serializable]
public class ST_Task_Print : ST_Task
{
    public string printText;

    public override void OnExecute()
    {
        Debug.Log(printText);
        OnExit();
    }
    public override void OnExit()
    {
        taskEnded = true;
    }
    public override void OnTick()
    {
        
    }
}
