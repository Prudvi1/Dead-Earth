using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour {

    //public methods
    public void SetStateMachine(AIStateMachine stateMachine) { stateMachine = _stateMachine; }

    //Default Handlers
    public virtual void OnEnterState()          { }
    public virtual void OnExitState()           { } 
    public virtual void OnAnimatorUpdated()     { }
    public virtual void OnAnimatorIKUpdated()   { }
    public virtual void OnTriggerEvent(AITriggerEventType eventType, Collider other)        { }
    public virtual void OnDestinatioReached(bool isReached) { }

    //Abstract Methods
    public abstract AIStateType GetStateType();
    public abstract AIStateType OnUpdate();

    protected AIStateMachine _stateMachine;
}
