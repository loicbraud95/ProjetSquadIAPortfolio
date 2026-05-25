using UnityEngine;
using UnityEngine.Events;

public class State : MonoBehaviour
{
    public virtual void EnterState() { }
    public virtual void ExitState() {  }
    public virtual void UpdateState() { }
}
