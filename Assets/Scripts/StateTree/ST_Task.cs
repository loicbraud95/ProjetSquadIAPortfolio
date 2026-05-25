using System;
using UnityEngine;

[Serializable]
public abstract class ST_Task
{
    [HideInInspector]
    public bool taskEnded = false;

    [HideInInspector]
    public CustomAgent agent;

    public abstract void OnExecute();
    public abstract void OnExit();
    public abstract void OnTick();
}
