using System.Collections.Generic;
using UnityEngine;

public class StateMachine 
{
    private  IState _currentState;
    public  IState CurrentState => _currentState;
    
    Dictionary<System.Type,IState> states;

    public StateMachine(IState currentState)
    {
        states = new Dictionary<System.Type,IState>();
        states.Add(currentState.GetType(),currentState);
        _currentState = currentState;
    }

    public void AddState(IState state)
    {
        if(states.ContainsKey(state.GetType())) return;
        states.Add(state.GetType(), state);
    }

    public void RemoveState(IState state)
    {
        states.Remove(state.GetType());
    }

    public void TransiteToState(System.Type newStateType)
    {
        if (!states.ContainsKey(newStateType))
        {
            Debug.LogAssertion($"状态机找不到状态{newStateType.Name}");
            return;
        }
        if (_currentState != null)
        {
            _currentState.OnExit();
        }
        _currentState = states[newStateType];
        _currentState.OnEnter();
    }

    public void OnUpdate()
    {
        _currentState.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        _currentState.OnFixedUpdate();
    }
    
}
