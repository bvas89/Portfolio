
using UnityEngine;

public class StateFactory<T>
{
    private T Owner;
    private FSMState<T> CurrentState;
    public FiniteStateMachine<T> StateMachine;
    public StateFactory(FiniteStateMachine<T> currentContext) { StateMachine = currentContext; }
    public States States;

    public void Initialize(T owner, States sts)
    {
        CurrentState = null;
        Owner = owner;
        States = sts;
    }

    public void Update()
    {
        if (CurrentState != null) CurrentState.Execute(Owner);
    }

    public void ChangeState(FSMState<T> NewState)
    {
        if(CurrentState != null)
        CurrentState.Exit(Owner);
        CurrentState = NewState;
        CurrentState.Enter(Owner);
    }

    public void DestroyFactory()
    {
        if (CurrentState != null)
            CurrentState.Exit(Owner);
    }
}