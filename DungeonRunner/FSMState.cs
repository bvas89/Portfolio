using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FSMState<T>
{
    public FiniteStateMachine<T> Machine;
    public StateFactory<T> Factory;

    public FSMState(StateFactory<T> factory, FiniteStateMachine<T> fsmT)
    { this.Machine = fsmT; this.Factory = factory; }

    abstract public void Enter(T t);
    abstract public void Execute(T t);
    abstract public void Exit(T t);

    public virtual void ChangeState(FSMState<T> newState)
    {
        Factory.ChangeState(newState);
    }

    public States States => GetStates(Factory);
    private States GetStates(StateFactory<T> factory)
    {
        return factory.States;
    }

    protected float timer;
    protected int index;
}