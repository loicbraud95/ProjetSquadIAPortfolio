using UnityEngine;

public abstract class ICondition : ScriptableObject
{
    public abstract bool Evaluate(CustomAgent agent);
}


