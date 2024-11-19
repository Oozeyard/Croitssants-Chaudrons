using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachineState
{
    public List<StateMachineState> transitions;
    public Action Enter;
    public Action Exit;
    public Func<int> Update;
};

public class StateMachine
{
    private StateMachineState currentState;

    public void Step()
    {
        if (currentState != null)
        {
            int nextState = currentState.Update();
            if (nextState != -1)
            {
                if (currentState.Exit != null)
                    currentState.Exit();
                currentState = currentState.transitions[nextState];
                if (currentState.Enter != null)
                    currentState.Enter();
            }
        }
    }

    public void SetState(StateMachineState state)
    {
        if (currentState != null && currentState.Exit != null)
        {
            currentState.Exit();
        }
        currentState = state;
        if (currentState.Enter != null)
            currentState.Enter();
    }
};