using System.Collections.Generic;
using Godot;

public class FiniteStateMachine
{
    protected Dictionary<string, State> states = new();
    public State CurrentState { get; private set; }
    public string CurrentStateName { get; private set; }
    public string PreviousStateName { get; set; }
    public void ExecuteStatePhysics(float delta) => CurrentState.PhysicsProcess(delta);
    public void ExecuteStateProcess(float delta) => CurrentState.Process(delta);
    public void AddState(string key, State state)
    {
        states[key] = state;
        state.StateMachine = this;
    }
    public void ChangeState(string newState)
    {
        if (CurrentState != null) CurrentState.Exit();
        var nextState = states[newState];
        nextState.Enter(CurrentState);
        CurrentState = nextState;
        CurrentStateName = newState;
        GD.Print("changed state to " + CurrentStateName);
    }
}