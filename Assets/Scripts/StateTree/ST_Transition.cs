using System;

[Serializable]
public class ST_Transition
{
    public ST_State TargetState;
    public ICondition Condition;
}
